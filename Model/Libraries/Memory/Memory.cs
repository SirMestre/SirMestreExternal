using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Model.Libraries.Memory
{
    public class Memory
    {
        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(IntPtr Handle, long Address, byte[] buffer, int Size, int BytesWritten = 0);
        [DllImport("kernel32.dll")]
        public static extern int ReadProcessMemory(IntPtr Handle, long Address, byte[] buffer, int Size, int BytesRead = 0);

        public IntPtr pHandle;
        public string ExeName { get; set; }
        public string ProcessName { get; set; }
        public long BaseAddress { get; set; }

        public IntPtr GetProcessHandle()
        {
            try
            {
                Process[] ProcList = Process.GetProcessesByName(ExeName);

                pHandle = ProcList[0].Handle;

                return pHandle;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public long GetBaseAddress(string ModuleName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(ExeName);
                ProcessModuleCollection modules = processes[0].Modules;
                ProcessModule DLLBaseAddress = null;

                foreach (ProcessModule i in modules)
                {
                    if (i.ModuleName == ModuleName)
                    {
                        DLLBaseAddress = i;
                    }
                }

                return DLLBaseAddress.BaseAddress.ToInt64();
            }
            catch
            {
                return 0;
            }
        }

        public long GetPointerAddress(long Pointer, int[] Offset = null)
        {
            byte[] Buffer = new byte[8];

            ReadProcessMemory(GetProcessHandle(), Pointer, Buffer, Buffer.Length);

            if (Offset != null)
            {
                for (int x = 0; x < (Offset.Length - 1); x++)
                {
                    Pointer = BitConverter.ToInt64(Buffer, 0) + Offset[x];
                    ReadProcessMemory(GetProcessHandle(), Pointer, Buffer, Buffer.Length);
                }

                Pointer = BitConverter.ToInt64(Buffer, 0) + Offset[Offset.Length - 1];
            }

            return Pointer;
        }

        public void WriteBytes(long Address, byte[] Bytes)
        {
            WriteProcessMemory(GetProcessHandle(), Address, Bytes, Bytes.Length);
        }
        public void WriteFloat(long Address, float Value)
        {
            WriteProcessMemory(GetProcessHandle(), Address, BitConverter.GetBytes(Value), 4);
        }
        public void WriteDouble(long Address, double Value)
        {
            WriteProcessMemory(GetProcessHandle(), Address, BitConverter.GetBytes(Value), 8);
        }
        public void WriteInteger(long Address, int Value, int size)
        {
            WriteProcessMemory(GetProcessHandle(), Address, BitConverter.GetBytes(Value), size);
        }
        public void WriteString(long Address, string String)
        {
            byte[] Buffer = new ASCIIEncoding().GetBytes(String);
            WriteProcessMemory(GetProcessHandle(), Address, Buffer, Buffer.Length);
        }

        public byte[] ReadBytes(long Address, int Length)
        {
            byte[] Buffer = new byte[Length];
            ReadProcessMemory(GetProcessHandle(), Address, Buffer, Length);
            return Buffer;
        }
        public float ReadFloat(long Address)
        {
            byte[] Buffer = new byte[4]; ;
            ReadProcessMemory(GetProcessHandle(), Address, Buffer, 4);
            return BitConverter.ToSingle(Buffer, 0);
        }
        public double ReadDouble(long Address)
        {
            byte[] Buffer = new byte[8];
            ReadProcessMemory(GetProcessHandle(), Address, Buffer, 8);
            return BitConverter.ToDouble(Buffer, 0);
        }
        public int ReadInteger(long Address, int Length)
        {
            byte[] Buffer = new byte[Length];
            ReadProcessMemory(GetProcessHandle(), Address, Buffer, Length);
            return BitConverter.ToInt32(Buffer, 0);
        }
        public string ReadString(long Address, int size)
        {
            byte[] Buffer = new byte[size]; ;
            ReadProcessMemory(GetProcessHandle(), Address, Buffer, size);
            return new ASCIIEncoding().GetString(Buffer);
        }
        public long ReadPointer(long Address)
        {
            byte[] Buffer = new byte[8];
            ReadProcessMemory(GetProcessHandle(), Address, Buffer, Buffer.Length);
            return BitConverter.ToInt64(Buffer, 0);
        }
    }
}
