using System;
using System.IO;

namespace TheRuleOfSilvester.Core
{
    public interface IPlayer
    {
        char Avatar { get; }
        string Name { get; set; }
        IBaseRole Role { get; }
        
        void Deserialize(BinaryReader binaryReader);
        void MoveDown(bool ghostMode);
        void MoveLeft(bool ghostMode);
        void MoveRight(bool ghostMode);
        void MoveUp(bool ghostMode);
        void Serialize(BinaryWriter binaryWriter);
        void SetAvatar(char avatar);
        void StartAction();
    }
}