using SirMestreBlackCat.Model;
using MahApps.Metro.Controls;
using Model.Libraries.KeyBoardHooking;
using System.Threading;
using System.Windows;

namespace WpfApplication
{
    public partial class MainWindow : MetroWindow
    {
        MemoryFunctions MemoryFunctions = new MemoryFunctions("GTA5", "GTA5.exe");

        public MainWindow()
        {
            InitializeComponent();
            new KeyBoardHooking();  
        }

        private void God_Mode_ToggleSwitch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MemoryFunctions.GAME_set_God_Mode(God_Mode_ToggleSwitch.IsChecked);
                });
            } else
            {
                God_Mode_ToggleSwitch.IsChecked = false;
            }
        }

        private int Wanted_Level = 0;
        private void Wanted_Level_NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            int Wanted_Level_Selected = (int)Wanted_Level_NumericUpDown.Value;
            if (Wanted_Level_NumericUpDown.IsInitialized == true)
            {
                Wanted_Level = Wanted_Level_Selected;
                if (MemoryFunctions.IsGameRunning() == true)
                {
                    Thread Thread = new Thread(() =>
                    {
                        while (Wanted_Level == Wanted_Level_Selected)
                        {
                            if (MemoryFunctions.GAME_get_Wanted_Level() != Wanted_Level)
                            {
                                MemoryFunctions.GAME_set_Wanted_Level(Wanted_Level);
                            }
                            Thread.Sleep(100);
                        }
                    });
                    Thread.IsBackground = true;
                    Thread.SetApartmentState(ApartmentState.STA);
                    Thread.Start();
                }
                else
                {
                    Wanted_Level_NumericUpDown.Value = 0;
                }
            }
        }

        private void Sprint_Speed_Slider_DragCompleted(object sender, RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Thread Thread = new Thread(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MemoryFunctions.GAME_set_Sprint_Speed((float)Sprint_Speed_Slider.Value);
                    });
                });
                Thread.IsBackground = true;
                Thread.SetApartmentState(ApartmentState.STA);
                Thread.Start();
            }
            else
            {
                Sprint_Speed_Slider.Value = 1;
            }
        }

        private void Swim_Speed_Slider_DragCompleted(object sender, RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Thread Thread = new Thread(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MemoryFunctions.GAME_set_Swim_Speed((float)Swim_Speed_Slider.Value);
                    });
                });
                Thread.IsBackground = true;
                Thread.SetApartmentState(ApartmentState.STA);
                Thread.Start();
            }
            else
            {
                Swim_Speed_Slider.Value = 1;
            }
        }

        private void Unlimited_Ammo_ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MemoryFunctions.GAME_set_Unlimited_Ammo(Unlimited_Ammo_ToggleSwitch.IsChecked);
                });
            }
            else
            {
                Unlimited_Ammo_ToggleSwitch.IsChecked = false;
            }
        }

        private void Unlimited_Magazine_ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MemoryFunctions.GAME_set_Unlimited_Magazine(Unlimited_Magazine_ToggleSwitch.IsChecked);
                });
            }
            else
            {
                Unlimited_Magazine_ToggleSwitch.IsChecked = false;
            }
        }

        private void God_Mode_Vehicle_ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MemoryFunctions.GAME_set_God_Mode_Vehicle(God_Mode_ToggleSwitch.IsChecked);
                });
            } else
            {
                God_Mode_Vehicle_ToggleSwitch.IsChecked = false;
            }
        }

        private void No_Bike_Fall_ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    MemoryFunctions.GAME_set_No_Bike_Fall(No_Bike_Fall_ToggleSwitch.IsChecked);
                });
            }
            else
            {
                No_Bike_Fall_ToggleSwitch.IsChecked = false;
            }
        }

        private void Teleport_To_Waypoint_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryFunctions.IsGameRunning() == true)
            {
                Thread Thread = new Thread(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                    Teleport_To_Waypoint_Button.IsEnabled = false;
                        Teleport_To_Waypoint_Button.Content = "Teleporting...";
                    });
                    MemoryFunctions.GAME_teleport_to_Waypoint();
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Teleport_To_Waypoint_Button.IsEnabled = true;
                        Teleport_To_Waypoint_Button.Content = "Teleport to Waypoint";
                    });
                });
                Thread.IsBackground = true;
                Thread.SetApartmentState(ApartmentState.STA);
                Thread.Start();
            }
        }
    }
}
