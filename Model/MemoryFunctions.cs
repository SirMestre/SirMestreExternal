using Model.Libraries.Memory;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SirMestreBlackCat.Model
{
    public class MemoryFunctions : Memory
    {
        public enum GAME_VERSION_DETECTED
        {
            STEAM,
            SOCIALCLUB
        }

        public GAME_VERSION_DETECTED GAME_VERSION;

        int WorldPTR;
        int AmmoPTR;
        int ClipPTR;
        int BlipPTR;

        int WorldPTR_SOCIALCLUB = 0x2366EC8;
        int AmmoPTR_SOCIALCLUB = 0xE88EB9;
        int ClipPTR_SOCIALCLUB = 0xE88E74;
        int BlipPTR_SOCIALCLUB = 0x1F9E750;

        int WorldPTR_STEAM = 0x236ADE0;
        int AmmoPTR_STEAM = 0xE89425;
        int ClipPTR_STEAM = 0xE893E0;
        int BlipPTR_STEAM = 0x1F9A2C0;


        int[] OFFSETS_God_Mode = new int[] { 0x08, 0x189 };
        int[] OFFSETS_God_Mode_Vehicle = new int[] { 0x08, 0xD28, 0x189 };
        int[] OFFSETS_No_Bike_Fall = new int[] { 0x8, 0x13EC };
        int[] OFFSETS_Wanted_Level = new int[] { 0x08, 0x10B8, 0x7F8 };
        int[] OFFSETS_Sprint_Speed = new int[] { 0x08, 0x10B8, 0x14C };
        int[] OFFSETS_Swim_Speed = new int[] { 0x08, 0x10B8, 0x0148 };

        public MemoryFunctions(string exeName, string processName)
        {
            ExeName = exeName;
            ProcessName = processName;
            BaseAddress = GetBaseAddress(ProcessName);
            pHandle = GetProcessHandle();
        }

        // God Mode.
        public byte[] GAME_get_God_Mode()
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_God_Mode);
            return ReadBytes(pointer, 2);
        }
        public void GAME_set_God_Mode(bool? enabled)
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_God_Mode);
            if (enabled == true)
            {
                WriteBytes(pointer, new byte[] { 0x1, 0x69 });
            }
            else
            {
                WriteBytes(pointer, new byte[] { 0x0, 0x69 });
            }
        }

        // God Mode Vehicle.
        public void GAME_set_God_Mode_Vehicle(bool? enabled)
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_God_Mode_Vehicle);
            if (enabled == true)
            {
                WriteBytes(pointer, new byte[] { 0x1 });
            }
            else
            {
                WriteBytes(pointer, new byte[] { 0x0 });
            }
        }

        // No Bike Fall.
        public void GAME_set_No_Bike_Fall(bool? enabled)
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_No_Bike_Fall);
            if (enabled == true)
            {
                WriteBytes(pointer, new byte[] { 0xC9 });
            }
            else
            {
                WriteBytes(pointer, new byte[] { 0xC8 });
            }
        }

        // Wanted Level.
        public int GAME_get_Wanted_Level()
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_Wanted_Level);
            return ReadInteger(pointer, 4);
        }

        public void GAME_set_Wanted_Level(int value)
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_Wanted_Level);
            WriteInteger(pointer, value, 4);
        }

        // Sprint Speed.
        public void GAME_set_Sprint_Speed(float value)
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_Sprint_Speed);
            WriteFloat(pointer, value);
        }

        // Swim Speed.
        public void GAME_set_Swim_Speed(float value)
        {
            long pointer = GetPointerAddress(BaseAddress + WorldPTR, OFFSETS_Swim_Speed);
            WriteFloat(pointer, value);
        }

        // Unlimited Ammo.
        public void GAME_set_Unlimited_Ammo(bool? enabled)
        {
            long pointer = GetPointerAddress(BaseAddress + AmmoPTR);

            if (enabled == true)
            {
                WriteBytes(pointer, BitConverter.GetBytes(0xE8909090));
            }
            else
            {
                WriteBytes(pointer, BitConverter.GetBytes(0xE8d12b41));
            }
        }

        // Unlimited Magazine.
        public void GAME_set_Unlimited_Magazine(bool? enabled)
        {

            long pointer = GetPointerAddress(BaseAddress + ClipPTR);

            if (enabled == true)
            {
                WriteBytes(pointer, BitConverter.GetBytes(0x3b909090));
            }
            else
            {
                WriteBytes(pointer, BitConverter.GetBytes(0x3bc92b41));
            }
        }

        // Teleport to Waypoint.
        public void GAME_teleport_to_Waypoint()
        {
            for (var i = 0; i < 0x800; i++)
            {
                long pointer = GetPointerAddress(BaseAddress + BlipPTR);
                long address = ReadPointer(pointer + (i * 8));
                if (address > 0)
                {
                    if (ReadInteger(address + 0x40, 4) == 8 && ReadInteger(address + 0x48, 4) == 84)
                    {
                        float waypointposX = ReadFloat(address + 0x10);
                        float waypointposY = ReadFloat(address + 0x14);
                        long worldptr = GetPointerAddress(BaseAddress + WorldPTR);
                        long player = ReadPointer(ReadPointer(worldptr) + 8);
                        byte[] vehicle_or_not = ReadBytes(player + 0x146B, 1);
                        if (vehicle_or_not[0] == 0)
                        {
                            player = ReadPointer(player + 0xD28);
                        }
                        long vehicle = ReadPointer(player + 0x30);
                        WriteFloat(vehicle + 0x50, waypointposX);
                        WriteFloat(vehicle + 0x54, waypointposY);
                        WriteFloat(vehicle + 0x58, -210);
                        WriteFloat(player + 0x90, waypointposX);
                        WriteFloat(player + 0x94, waypointposY);
                        WriteFloat(player + 0x98, -210);
                    }
                }
            }
        }

        public bool IsGameRunning()
        {
            Process[] process = Process.GetProcessesByName(ExeName);
            if (process.Length > 0)
            {
                string process_path = process[0].MainModule.FileName;
                FileInfo FileInfo = new FileInfo(process_path);
                if (FileInfo.Length == 60218776)
                {
                    GAME_VERSION = GAME_VERSION_DETECTED.SOCIALCLUB;
                }
                else
                {
                    GAME_VERSION = GAME_VERSION_DETECTED.STEAM;
                }

                if (GAME_VERSION == GAME_VERSION_DETECTED.SOCIALCLUB)
                {
                    WorldPTR = WorldPTR_SOCIALCLUB;
                    AmmoPTR = AmmoPTR_SOCIALCLUB;
                    ClipPTR = ClipPTR_SOCIALCLUB;
                    BlipPTR = BlipPTR_SOCIALCLUB;
                }
                else
                {
                    WorldPTR = WorldPTR_STEAM;
                    AmmoPTR = AmmoPTR_STEAM;
                    ClipPTR = ClipPTR_STEAM;
                    BlipPTR = BlipPTR_STEAM;
                }
                return true;
            }
            else
            {
                MessageBox.Show("ERROR : You need to launch " + ExeName, "ERROR");
                return false;
            }
        }
    }
}
