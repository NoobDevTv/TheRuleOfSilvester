using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheRuleOfSilvester.Core.IoC
{
    public sealed class StandaloneTypeContainer : ITypeContainer
    {

        private readonly Dictionary<Type, TypeInformation> typeInformationRegister;
        private readonly Dictionary<Type, Type> typeRegister;
        private readonly HashSet<TypeInformation> uncompletedList;
        private readonly SemaphoreExtended localSemaphore;

        public StandaloneTypeContainer()
        {
            typeInformationRegister = new Dictionary<Type, TypeInformation>();
            typeRegister = new Dictionary<Type, Type>();
            uncompletedList = new HashSet<TypeInformation>();
            localSemaphore = new SemaphoreExtended(1, 1);
        }

        public void Register(Type registrar, Type type, InstanceBehaviour instanceBehaviour)
        {
            TypeInformation registerInfo = null;
            if (!typeInformationRegister.ContainsKey(type))
            {
                registerInfo = new TypeInformation(this, type, instanceBehaviour);
                typeInformationRegister.Add(type, registerInfo);
            }

            typeRegister.Add(registrar, type);

            var removelist = new List<TypeInformation>();
            foreach (var typeInformation in uncompletedList)
            {
                typeInformation.RecreateUncompleteCtors();
                if (typeInformation.Completed)
                    removelist.Add(typeInformation);
            }

            uncompletedList.RemoveWhere(t => removelist.Contains(t));
            if (registerInfo != null && !registerInfo.Completed)
                uncompletedList.Add(registerInfo);
        }
        public void Register<T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => Register(typeof(T), typeof(T), instanceBehaviour);
        public void Register<TRegistrar, T>(InstanceBehaviour instanceBehaviour = InstanceBehaviour.Instance) where T : class
            => Register(typeof(TRegistrar), typeof(T), instanceBehaviour);
        public void Register(Type registrar, Type type, object singelton)
        {
            if (!typeInformationRegister.ContainsKey(type))
                typeInformationRegister.Add(type, new TypeInformation(this, type, InstanceBehaviour.Singleton, singelton));

            typeRegister.Add(registrar, type);

            foreach (var typeInformation in typeInformationRegister.Values.Where(t => !t.Completed))
            {
                typeInformation.RecreateUncompleteCtors();
            }
        }
        public void Register<T>(T singelton) where T : class
            => Register(typeof(T), typeof(T), singelton);
        public void Register<TRegistrar, T>(object singelton) where T : class
            => Register(typeof(TRegistrar), typeof(T), singelton);

        public bool TryResolve(Type type, out object instance)
        {
            instance = GetOrNull(type);
            return instance != null;
        }
        public bool TryResolve<T>(out T instance) where T : class
        {
            var result = TryResolve(typeof(T), out var obj);
            instance = (T)obj;
            return result;
        }

        public object Get(Type type)
            => GetOrNull(type) ?? throw new KeyNotFoundException($"Type {type} was not found in Container");

        public T Get<T>() where T : class
            => (T)Get(typeof(T));

        public object GetOrNull(Type type)
        {
            if (typeRegister.TryGetValue(type, out var searchType))
            {
                if (typeInformationRegister.TryGetValue(searchType, out var typeInformation))
                    return typeInformation.Instance;
            }
            return null;
        }
        public T GetOrNull<T>() where T : class
            => (T)GetOrNull(typeof(T));

        public object GetUnregistered(Type type)
            => GetOrNull(type)
                ?? CreateObject(type)
                ?? throw new InvalidOperationException($"Can not create unregistered type of {type}");

        public T GetUnregistered<T>() where T : class
            => (T)GetUnregistered(typeof(T));

        public object CreateObject(Type type)
        {
            var tmpList = new List<object>();

            var constructors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                bool next = false;
                foreach (var parameter in constructor.GetParameters())
                {
                    if (TryResolve(parameter.ParameterType, out object instance))
                    {
                        tmpList.Add(instance);
                    }
                    else if (!parameter.IsOptional)
                    {
                        tmpList.Clear();
                        next = true;
                        break;
                    }
                }

                if (next)
                    continue;

                return constructor.Invoke(tmpList.ToArray());
            }

            if (constructors.Count() < 1)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        private IEnumerable<CtorInformation> GetCtorInformations(Type type)
        {
            var constructors = type
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length);

            foreach (var constructor in constructors)
            {
                var info = new CtorInformation(constructor);
                BuildCtorInformation(info);

                yield return info;
            }
        }

        private void BuildCtorInformation(CtorInformation info)
        {
            var func = info.Length > 0 ? (Action<ParameterInfo, TypeInformation>)info.Update : info.Add;
            foreach (var parameter in info.GetParameters())
            {
                if (typeRegister.TryGetValue(parameter.ParameterType, out var searchType)
                   && typeInformationRegister.TryGetValue(searchType, out var typeInformation))
                {
                    func(parameter, typeInformation);
                }
                else
                {
                    func(parameter, null);
                }
            }
        }

        public T CreateObject<T>() where T : class
            => (T)CreateObject(typeof(T));

        public void Dispose()
        {
            typeRegister.Clear();
            typeInformationRegister.Values
                .Where(t => t.Behaviour == InstanceBehaviour.Singleton && t.Instance != this)
                .Select(t => t.Instance as IDisposable)
                .ToList()
                .ForEach(i => i?.Dispose());

            typeInformationRegister.Clear();
        }

        private class TypeInformation
        {
            public InstanceBehaviour Behaviour { get; set; }
            public object Instance => CreateObject();

            public bool Completed { get; private set; }

            private readonly StandaloneTypeContainer typeContainer;
            private readonly Type type;
            private object singeltonInstance;
            private readonly List<CtorInformation> ctors;

            public TypeInformation(StandaloneTypeContainer container,
                Type type, InstanceBehaviour instanceBehaviour, object instance = null)
            {
                this.type = type;
                Behaviour = instanceBehaviour;
                typeContainer = container;
                singeltonInstance = instance;
                ctors = container
                    .GetCtorInformations(type)
                    .OrderByDescending(ctor => ctor.Length)
                    .ToList();

                Completed = !ctors.Any(c => !c.IsComplete);
            }

            private object CreateObject()
            {
                if (Behaviour == InstanceBehaviour.Singleton && singeltonInstance != null)
                    return singeltonInstance;

                var obj = ctors.FirstOrDefault(c => c.IsComplete)?.Invoke();

                if (Behaviour == InstanceBehaviour.Singleton)
                {
                    singeltonInstance = obj;
                    Completed = true;
                }

                return obj;
            }

            public void RecreateUncompleteCtors()
            {
                if (Completed)
                    return;

                foreach (var ctor in ctors.Where(ctor => !ctor.IsComplete))
                    typeContainer.BuildCtorInformation(ctor);

                Completed = !ctors.Any(c => !c.IsComplete);
            }
        }

        private class CtorInformation
        {
            public bool IsComplete { get; private set; }
            public int Length => parameters.Count;

            private readonly ConstructorInfo constructor;
            private readonly Dictionary<ParameterInfo, TypeInformation> parameters;

            public CtorInformation(ConstructorInfo constructor)
            {
                parameters = new Dictionary<ParameterInfo, TypeInformation>();
                this.constructor = constructor;
                IsComplete = true;
            }

            internal void Add(ParameterInfo parameter, TypeInformation typeInformation)
            {
                parameters.Add(parameter, typeInformation);

                if (typeInformation == null && !parameter.IsOptional)
                    IsComplete = false;
            }

            internal void Update(ParameterInfo parameter, TypeInformation typeInformation)
            {
                parameters[parameter] = typeInformation;

                if (typeInformation == null && !parameter.IsOptional)
                    IsComplete = false;
                else
                    IsComplete = !parameters.Any(info => info.Value == null && !info.Key.IsOptional);
            }

            internal void Clear()
            {
                parameters.Clear();
            }

            internal IEnumerable<ParameterInfo> GetParameters()
                => constructor.GetParameters();

            internal object Invoke()
            {
                if (!IsComplete)
                    throw new InvalidOperationException();

                return constructor.Invoke(parameters
                    .OrderBy(v => v.Key.Position)
                    .Select(v => v.Value)
                    .Select(info =>
                    {
                        if (info == null)
                            return null;
                        else
                            return info.Instance;
                    }).ToArray());
            }
        }
    }
}
