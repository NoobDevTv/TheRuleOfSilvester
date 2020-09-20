using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TheRuleOfSilvester.Generators
{
    [Generator]
    public class VariantGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var generations = 20;
            var builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            //builder.AppendLine("using System.Reactive.Linq;");
            builder.AppendLine("namespace TheRuleOfSilvester.Core.SumTypes {");
            //for (int i = 2; i < generations; i++)
            //builder.AppendLine("public class Variant<T1>{}");
            for (int i = 2; i <= generations; i++)
            {
                var generics = Enumerable.Range(1, i);
                var typeParams = generics.Select(index => "T" + index).ToArray();
                var paramsList = string.Join(", ", typeParams);

                builder.AppendLine($"public class Variant<{paramsList}> : IEquatable<Variant<{paramsList}>> {{");
                builder.AppendLine("public Type ValueType => value.GetType();");
                builder.AppendLine("private readonly object value;");
                builder.AppendLine("private readonly int typeId;");

                for (int index = 0; index < typeParams.Length; index++)
                {
                    builder.AppendLine($"public Variant({typeParams[index]} value) => (typeId, this.value) = ({index}, value);");

                    //builder.AppendLine($"public bool Is<{typeParams[index]}>() => {index} == typeId;");
                }

                AddGetMethod(builder, typeParams);
                AddMapMethod(builder, typeParams);

                builder.AppendLine($"public override bool Equals(object other) => other is Variant<{paramsList}> variant && Equals(variant);");
                builder.AppendLine($"public bool Equals(Variant<{paramsList}> other)");
                builder.AppendLine($"=> other != null");
                builder.AppendLine($"   && typeId == other.typeId");
                builder.AppendLine($"   && value.Equals(other.value);");

                builder.AppendLine($"public override int GetHashCode() => typeId + -1851467485 + (value?.GetHashCode() ?? 1);");

                foreach (var t in typeParams)
                    builder.AppendLine($"public static implicit operator Variant<{paramsList}>({t} obj) => new Variant<{paramsList}>(obj);");

                builder.AppendLine($"public static bool operator ==(Variant<{paramsList}> left, Variant<{paramsList}> right) => left.Equals(right);");
                builder.AppendLine($"public static bool operator !=(Variant<{paramsList}> left, Variant<{paramsList}> right) => !(left == right);");

                builder.AppendLine("}");

                GenerateIEnumerableExtensions(builder, typeParams);
                GenerateIObservableExtensions(builder, typeParams);
            }

            builder.AppendLine("}");

            context.AddSource("variantGenerator", SourceText.From(builder.ToString(), Encoding.UTF8));

        }

        private void GenerateIEnumerableExtensions(StringBuilder builder, string[] typeParams)
        {
            //var paramsList = string.Join(", ", typeParams);
            //builder.AppendLine("public static partial class IEnumerableExtensions {");

            //for (int i = 0; i < typeParams.Length; i++)
            //{
            //    var parameter = typeParams[i];
            //    builder.AppendLine($"public static IEnumerable<TResult> Map(this IEnumerable<Variant<{paramsList}>> enumerable, Func<{parameter}, TResult> select){{");
            //    builder.AppendLine($"foreach(var value in enumerable)");
            //    builder.AppendLine("    yield return value.Get(selector);");
            //    builder.AppendLine("}");
            //}

            //builder.AppendLine($"public bool Map({string.Join(",", typeParams.Select(x => $"Action<{x}> action{x}"))}){{");
            //builder.AppendLine($"switch(typeId){{");
            //typeParams.ForEach((t, i) => builder.AppendLine($"case {i}: action{t}(value); break;"));
            //builder.AppendLine($"}}");
            //builder.AppendLine($"}}");

            //builder.AppendLine("}");
        }

        private void GenerateIObservableExtensions(StringBuilder builder, string[] typeParams)
        {
            builder.AppendLine("public static partial class IObservableExtensions {");
            builder.AppendLine("}");
        }

        private void AddMapMethod(StringBuilder builder, string[] typeParams)
        {
            for (int i = 0; i < typeParams.Length; i++)
            {
                var parameter = typeParams[i];
                builder.AppendLine($"public void Map(Action<{parameter}> action){{");
                builder.AppendLine($"if ({i} != typeId)");
                builder.AppendLine("    throw new TypeMismatchException();");
                builder.AppendLine($"action(({parameter})value);}}");
            }

            builder.AppendLine($"public void Map({string.Join(",", typeParams.Select(x => $"Action<{x}> action{x}"))}){{");
            builder.AppendLine($"switch(typeId){{");
            for (int index = 0; index < typeParams.Length; index++)
                builder.AppendLine($"case {index}: action{typeParams[index]}(({typeParams[index]})value); break;");

            builder.AppendLine("default: throw new TypeMismatchException();");
            builder.AppendLine($"}}");
            builder.AppendLine($"}}");

        }

        private void AddGetMethod(StringBuilder builder, string[] typeParams)
        {

            for (int i = 0; i < typeParams.Length; i++)
            {
                var parameter = typeParams[i];
                builder.AppendLine($"public void Get(out {parameter} value){{");
                builder.AppendLine($"if ({i} != typeId)");
                builder.AppendLine("    throw new TypeMismatchException();");
                builder.AppendLine($"value = ({parameter})this.value;}}");

                builder.AppendLine($"public bool TryGet(out {parameter} value){{");
                builder.AppendLine($"value = default;");
                builder.AppendLine($"if ({i} != typeId)");
                builder.AppendLine("    return false;");
                builder.AppendLine($"value = ({parameter})this.value;");
                builder.AppendLine($"return true;}}");
            }

        }


        public void Initialize(GeneratorInitializationContext context) { }
    }
}
