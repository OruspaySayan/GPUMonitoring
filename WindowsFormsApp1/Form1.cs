using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        [DllImport("nvapi.dll", EntryPoint = "nvapi_QueryInterface", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr QueryInterface(uint offset);

        [DllImport("C:\\Users\\Sayan\\source\\repos\\Project2\\Debug\\Project2.dll", EntryPoint = "getCurrentTemp", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCurrentTemp(IntPtr funcAddress, int handle);

        [DllImport("C:\\Users\\Sayan\\source\\repos\\Project2\\Debug\\Project2.dll", EntryPoint = "getCurrentPstate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCurrentPstate(IntPtr funcAddress, int handle, ref uint currentPstate);

        [DllImport("C:\\Users\\Sayan\\source\\repos\\Project2\\Debug\\Project2.dll", EntryPoint = "getPstatesInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getPstatesInfo(IntPtr funcAddress, int handle, uint currentPstate, int[]buffer);

        [DllImport("C:\\Users\\Sayan\\source\\repos\\Project2\\Debug\\Project2.dll", EntryPoint = "getDynamicPstatesInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getDynamicPstatesInfo(IntPtr funcAddress, int handle, int[] buffer);

        int[] hdlGPU = new int[64];
        int nGPU = 0;

        public Form1()
        {
            InitializeComponent();
            Init();
            init();
            enumGPUs(hdlGPU, ref nGPU);
            button1_Click(null, null);
        }

        IntPtr NvInitAddress;
        IntPtr NvUnloadAddress;
        IntPtr NvEnumGPUsAddress;
        IntPtr NvGetPstatesAddress;
        IntPtr NvGetThermalSettingsAddress;
        IntPtr NvGetCurrentPstatesAddress;
        IntPtr NvGetDynamicPstatesInfoAddress;
        NvInitDelegate init;
        NvUnloadDelegate unload;
        NvEnumGPUsDelegate enumGPUs;

        void Init()
        {
            NvInitAddress = QueryInterface(0x0150E828);
            NvUnloadAddress = QueryInterface(0xD22BDD7E);
            NvEnumGPUsAddress = QueryInterface(0xE5AC921F);
            NvGetPstatesAddress = QueryInterface(0x6FF81213);
            NvGetThermalSettingsAddress = QueryInterface(0xE3640A56);
            NvGetCurrentPstatesAddress = QueryInterface(0x927DA4F6);
            NvGetDynamicPstatesInfoAddress = QueryInterface(0x60DED2ED);
            init = (NvInitDelegate)Marshal.GetDelegateForFunctionPointer(NvInitAddress, typeof(NvInitDelegate));
            unload = (NvUnloadDelegate)Marshal.GetDelegateForFunctionPointer(NvUnloadAddress, typeof(NvUnloadDelegate));
            enumGPUs = (NvEnumGPUsDelegate)Marshal.GetDelegateForFunctionPointer(NvEnumGPUsAddress, typeof(NvEnumGPUsDelegate));
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int NvInitDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int NvUnloadDelegate();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int NvEnumGPUsDelegate(int[] handles, ref int count);

        private void button1_Click(object sender, EventArgs e)
        {
            uint currentPstate = 0;
            getCurrentPstate(NvGetCurrentPstatesAddress, hdlGPU[0], ref currentPstate);
            int[] buffer = new int[6];
            getPstatesInfo(NvGetPstatesAddress, hdlGPU[0], currentPstate, buffer);
            label1.Text = Convert.ToString(buffer[0]) + " MHz";
            label3.Text = Convert.ToString(buffer[1]) + " MHz";
            label5.Text = Convert.ToString(buffer[2]) + " MHz";
            label7.Text = Convert.ToString(getCurrentTemp(NvGetThermalSettingsAddress, hdlGPU[0])) + " C";
            label16.Text = Convert.ToString((float)buffer[4] / 1000000) + " V";
            getDynamicPstatesInfo(NvGetDynamicPstatesInfoAddress, hdlGPU[0], buffer);
            label14.Text = Convert.ToString(buffer[0]) + " %";
            label12.Text = Convert.ToString(buffer[1]) + " %";
            label10.Text = Convert.ToString(buffer[2]) + " %";
            label8.Text = Convert.ToString(buffer[3]) + " %";
        }
    }
}