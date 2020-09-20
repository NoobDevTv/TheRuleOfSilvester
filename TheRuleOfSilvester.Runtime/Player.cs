﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Runtime.Items;
using TheRuleOfSilvester.Runtime.Roles;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Runtime
{
    public class Player : PlayerCell, IPlayer
    {
        public string Name
        {
            get => name; set
            {
                if (value.Length > 20)
                    name = value.Substring(0, 20);
                else
                    name = value;
            }
        }
        public char Avatar { get; private set; }

        public IBaseRole Role { get; private set; }

        public event EventHandler<Cell> PlayerChangedCell;

        private readonly int moveSizeX;
        private readonly int moveSizeY;

        private bool ghostMode;
        private GhostPlayer ghost;
        private string name;

        public Player() : base(1, 1, null)
        {
            IsLocal = false;
            Color = Color.Yellow;
            Lines = new BaseElement[1, 1];
            Name = "Tim";
            Role = RoleManager.GetRandomRole();
            moveSizeX = 5;
            moveSizeY = 3;
        }
        public Player(Map map, BaseRole role) : base(1, 1, map)
        {
            Lines = new BaseElement[1, 1];
            IsLocal = true;
            moveSizeX = 5;
            moveSizeY = 3;
            Name = "Tim";
            ghostMode = false;
            Role = role;
            var random = new Random();

            GenerateInventory(map, random);

            map.TextCells.Add(new TextCell("Inventory:", map) { Position = new Position(0, (map.Height + 1) * 3 + 1) });

            SetAvatar(role.Avatar);
        }
        
        public void SetAvatar(char avatar)
        {
            Avatar = avatar;
            Lines[0, 0] = Avatar;
        }

        public void MoveUp(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveUp();
                return;
            }

            MoveGeneral(new Position(Position.X, Position.Y - moveSizeY));
        }

        public void MoveDown(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveDown();
                return;
            }

            MoveGeneral(new Position(Position.X, Position.Y + moveSizeY));
        }

        public void MoveLeft(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveLeft();
                return;
            }

            MoveGeneral(new Position(Position.X - moveSizeX, Position.Y));
        }

        public void MoveRight(bool ghostMode)
        {
            if (ghostMode)
            {
                ghost.MoveRight();
                return;
            }

            MoveGeneral(new Position(Position.X + moveSizeX, Position.Y));
        }

        public void StartAction()
        {
            //TODO: more functions
            MoveCell();
        }

        public override void Update(Game game)
        {
            var lastInput = game.InputAction;

            if (lastInput == null || !lastInput.Valid)
                return;

            if (ghostMode)
                Invalid = true;

            switch (lastInput.Type)
            {
                case InputActionType.Up:
                    MoveUp(ghostMode);
                    break;
                case InputActionType.Down:
                    MoveDown(ghostMode);
                    break;
                case InputActionType.Left:
                    MoveLeft(ghostMode);
                    break;
                case InputActionType.Right:
                    MoveRight(ghostMode);
                    break;
                case InputActionType.StartAction:
                    StartAction();
                    break;
                default:
                    break;
            }
        }

        private void GenerateInventory(Map map, Random random)
        {
            var cellTypes = map.MapGenerator?.CellTypes;

            for (int i = 0; i < 3; i++)
            {
                var cell = (MapCell)Activator.CreateInstance(cellTypes[random.Next(0, cellTypes.Count)], map, true);
                cell.Position = new Position(1 + i * 2, Map.Height + 2);
                CellInventory.Add(cell);

            }
        }

        public override void Serialize(BinaryWriter binaryWriter)
        {
            base.Serialize(binaryWriter);
            binaryWriter.Write(Avatar);
            binaryWriter.Write(Name);

            Role.Serialize(binaryWriter);

            binaryWriter.Write(Position.X);
            binaryWriter.Write(Position.Y);

            binaryWriter.Write(CellInventory.Count);

            foreach (MapCell cell in CellInventory)
                cell.Serialize(binaryWriter);
        }

        public override void Deserialize(BinaryReader binaryReader)
        {
            base.Deserialize(binaryReader);
            SetAvatar(binaryReader.ReadChar());
            Name = binaryReader.ReadString();

            Role = (BaseRole)Activator.CreateInstance(Type.GetType(binaryReader.ReadString()));

            Position = new Position(binaryReader.ReadInt32(), binaryReader.ReadInt32());

            var count = binaryReader.ReadInt32();

            for (int i = 0; i < count; i++)
                CellInventory.Add(SerializeHelper.DeserializeMapCell(binaryReader) as MapCell);

        }

        private void MoveCell()
        {
            if (!ghostMode)
            {
                ghostMode = true;
                ghost = new GhostPlayer(Map, this);
            }
            else
            {
                ghostMode = false;

                var changedCell = ghost.SelectedCell;
                if (!changedCell.Movable)
                {
                    changedCell.Invalid = true;
                    ghost.Dispose();
                    ghost = null;
                    return;
                }
                Map.Cells.Remove(changedCell);

                var inventoryCell = CellInventory.FirstOrDefault();
                CellInventory.Remove(inventoryCell);

                inventoryCell.Position = changedCell.Position;
                inventoryCell.Invalid = true;

                Map.Cells.Add(inventoryCell);

                changedCell.Position = new Position(5, Map.Height + 2);
                changedCell.Invalid = true;
                CellInventory.ForEach(x =>
                {
                    x.Position = new Position(x.Position.X - 2, x.Position.Y);
                    x.Invalid = true;
                    CellInventory.Remove(x);
                    CellInventory.Add(x);
                });
                CellInventory.Add(changedCell as MapCell);


                var cellsToNormalize = Map.Cells.Where(c =>
                        c.Position.X == inventoryCell.Position.X && c.Position.Y == inventoryCell.Position.Y - 1
                    || c.Position.X == inventoryCell.Position.X && c.Position.Y == inventoryCell.Position.Y + 1
                    || c.Position.X == inventoryCell.Position.X - 1 && c.Position.Y == inventoryCell.Position.Y
                    || c.Position.X == inventoryCell.Position.X + 1 && c.Position.Y == inventoryCell.Position.Y)
                    .Select(x => (MapCell)x);
                cellsToNormalize.ForEach(x => x.NormalizeLayering());


                ghost.Dispose();
                ghost = null;

                PlayerChangedCell?.Invoke(this, inventoryCell);
            }
        }

    }
}
