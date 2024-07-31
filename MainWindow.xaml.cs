using StarLight_Core.Authentication;
using StarLight_Core.Enum;
using StarLight_Core.Launch;
using StarLight_Core.Models.Authentication;
using StarLight_Core.Models.Launch;
using StarLight_Core.Models.Utilities;
using StarLight_Core.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Windows.Threading;
using MinecraftLaunch;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Download;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Classes.Models.Install;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Resolver;
using StarLight_Core.Installer;
using StarLight_Core.Models.Installer;

namespace ArrowLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isUsernameOK = true;
        public bool qzqd = false;

        private string configFilePath = @"ArrowLauncher\ArrowLauncherConfig.json";

        private void writeConfigFile(string a)
        {
            if (!Directory.Exists("ArrowLauncher"))
            {
                Directory.CreateDirectory("ArrowLauncher");
            }
            File.WriteAllText(configFilePath, a);
        }
        
        
        //config file (launcher)
        public class LauncherConfig()
        {
            public string username { get; set; }
            public string version { get; set; }

            public string Name { get; set; }

            public string Uuid { get; set; }

            public string AccessToken { get; set; }

            public string ClientToken { get; set; }

            public bool HiddenWindow { get; set; } = true;
        }

        public class GameConfig()
        {
            public bool IsVersionIsolation { get; set; } = true;
            public int JavaVersion { get; set; } = 0; //0自动, 17, 8 -> Java17, Java8
        }

        GameConfig TargetGC = new GameConfig();

        //private string ClientId = "7a1e928c-5709-46a0-803d-e1066c81ee79";
        private string ClientId = "e1e383f9-59d9-4aa2-bf5e-73fe83b15ba0";

        private LauncherConfig luco = new LauncherConfig();
        
        
        private void loadConfigFile()
        {
            if (File.Exists(configFilePath))
            {
                string jf = File.ReadAllText(configFilePath);
                Console.WriteLine(jf);
                luco = JsonSerializer.Deserialize<LauncherConfig>(
                    jf, new JsonSerializerOptions
                    {
                        //WriteIndented = true,
                        //PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                OfflineUsername.Text = luco.username;
                OfflineVersionsList.Text = luco.version;

                OnlineVersionsList.Text = luco.version;

                hidewindow.IsChecked = luco.HiddenWindow;
                
                // Console.WriteLine(luco.version);
                // Console.WriteLine(luco.username);
                // Console.WriteLine(luco.AccessToken);
                // Console.WriteLine(luco.ClientToken);
                // Console.WriteLine(luco.Uuid);
                // Console.WriteLine(luco.Name);


                if (!(String.IsNullOrEmpty(luco.Uuid) &&
                    String.IsNullOrEmpty(luco.Name) &&
                    String.IsNullOrEmpty(luco.AccessToken) &&
                    String.IsNullOrEmpty(luco.ClientToken)))
                {
                    OnlineLoginButton.Content = "重新登录";
                    loginStatus.Text = "微软登录完成";
                    TONULL.Visibility = Visibility.Visible;
                }

                if (!String.IsNullOrEmpty(luco.version))
                {
                    if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                            , new JsonSerializerOptions()).JavaVersion == 0)
                        CustomJava.Text = "Auto";
                    if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                            , new JsonSerializerOptions()).JavaVersion == 8)
                        CustomJava.Text = "Java8";
                    if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                            , new JsonSerializerOptions()).JavaVersion == 17)
                        CustomJava.Text = "Java17";
                }
                
                
                
                
            }
            else
            {
                Directory.CreateDirectory("ArrowLauncher");
                string jf = JsonSerializer.Serialize(luco);
                File.WriteAllText(configFilePath,jf);
                loadConfigFile();
            }
        }

        private int CustomJavaTextToInt(string t)
        {
            if (t.Equals("Java17")) return 17;
            if (t.Equals("Java8")) return 8;
            if (t.Equals("Auto")) return 0;

            return 0;
        }

        public async void initDownloadVersionList()
        {
            var a = await InstallUtil.GetGameCoresAsync();
            foreach (var b in a)
            {
                if (b.Type.Equals("release"))
                {
                    GameInstallList.Items.Add(b.Id);
                }
                
                //Console.WriteLine(b.Type);
            }
            
        }

        public MainWindow()
        {
            InitializeComponent();

            { //用户名合法?
                string username = OfflineUsername.Text;
                char[] cUsername = username.ToCharArray();

                for (int i = 0; i < cUsername.Length; i++)
                {
                    if (!((cUsername[i] >= 'a' && cUsername[i] <= 'z')
                          || (cUsername[i] >= 'A' && cUsername[i] <= 'Z')
                          || (cUsername[i] >= '0' && cUsername[i] <= '9')
                          || cUsername[i] == '_' || cUsername[i] == ' '))
                    {
                        isUsernameOK = false;
                        isUserNameOKVisible.Visibility = Visibility.Visible;
                        qzqds.Visibility = Visibility.Visible;
                        qzqds.IsChecked = false;
                    }
                }

                int space = 0;

                for (int i = 0; i < cUsername.Length; i++)
                {
                    if (username[i] == ' ')
                    {
                        space++;
                    }
                }

                if (!String.IsNullOrWhiteSpace(username))
                {
                    isUsernameOK = true;
                    isUserNameOKVisible.Visibility = Visibility.Hidden;
                    qzqds.Visibility = Visibility.Hidden;
                }
                else
                {
                    isUsernameOK = false;
                    isUserNameOKVisible.Visibility = Visibility.Visible;
                    qzqds.Visibility = Visibility.Visible;
                    qzqds.IsChecked = false;
                }
            }

            initDownloadVersionList();

            CustomJava.Items.Add("Java17");
            CustomJava.Items.Add("Java8");
            CustomJava.Items.Add("Auto");
            
            //读取版本列表
            var vs = GameCoreUtil.GetGameCores(@".minecraft").ToArray();
            foreach (var v in vs)
            {
                
                OfflineVersionsList.Items.Add(v.Id);
                OnlineVersionsList.Items.Add((v.Id));

                if (!Directory.Exists("ArrowLauncher\\GameConfigs"))
                {
                    Directory.CreateDirectory("ArrowLauncher\\GameConfigs");
                }
                
                string jf1 = JsonSerializer.Serialize(new GameConfig());
                if(!File.Exists($"ArrowLauncher\\GameConfigs\\{v.Id}.json"))
                    File.WriteAllText($"ArrowLauncher\\GameConfigs\\{v.Id}.json", jf1);

                
            }
            
            loadConfigFile();
        }

        private void offlineReporter(ProgressReport report)
        {
            VisibleVersion.Text = $"{ report.Description} ({ report.Percentage} %)";
            
        }

        static async Task WaitForWindowWithTitleAsync(int processId, long MinMemrory)
        {
            Process process = Process.GetProcessById(processId);
            while (true)
            {
                if (process != null)
                {
                    long virtualMemorySize = process.VirtualMemorySize64; // 虚拟内存
                    long workingSet = process.WorkingSet64; // 工作集（物理内存）

                    long TaskMemorySize = (virtualMemorySize + workingSet) / (1024*1024); //MB
                    if (TaskMemorySize >= MinMemrory) return;
                }

                await Task.Delay(100);
            }
        }

        private async void OfflineLaunchButton_Click(object sender, RoutedEventArgs e)
        {
            
            //MessageBox.Show(GameCoreUtil.GetGameCore("Server").JavaVersion.ToString(), "Wrong", MessageBoxButton.OK, MessageBoxImage.Information);
            //GameCoreUtil.GetGameCore("Server").JavaVersion //(JavaVersion

            if ((qzqd || isUsernameOK) && OfflineVersionsList.SelectedItem != null)
            {
                try
                {
                    OfflineQDTEXT.Text = "启动中";

                    TargetGC = JsonSerializer.Deserialize<GameConfig>(
                        File.ReadAllText($"ArrowLauncher\\GameConfigs\\{OfflineVersionsList.Text}.json"));

                    var auth = new OfflineAuthentication(OfflineUsername.Text).OfflineAuth();
                    var lc = new LaunchConfig()
                    {
                        Account = new Account
                        {
                            BaseAccount = new BaseAccount
                            {
                                Name = OfflineUsername.Text,
                                Uuid = Guid.NewGuid().ToString()
                            }
                        },

                        JavaConfig = new JavaConfig
                        {
                            JavaPath = getJavaPathFromCustomJava(TargetGC.JavaVersion), //JavaPath
                            MaxMemory = 6198,
                            MinMemory = 1024
                        },

                        GameCoreConfig = new GameCoreConfig
                        {
                            Root = ".minecraft",
                            IsVersionIsolation = TargetGC.IsVersionIsolation,
                            Version = OfflineVersionsList.Text //版本
                        },

                        


                    };

                    MinecraftLauncher ml = new MinecraftLauncher(lc);
                    var la = await ml.LaunchAsync(offlineReporter);

                    VisibleVersion.Text = string.Concat("Minecraft ", OfflineVersionsList.SelectedItem.ToString());
                    VisibleVersion.Text = $"启动游戏中 (80 %)";

                    //await WaitForWindowWithTitleAsync(la.Process.Id, ml.JavaConfig.MinMemory);
                    await Task.Delay(1000);
                    //给点延迟qwq
                    
                    VisibleVersion.Text = $"启动完成 (100 %)";
                    await Task.Delay(1000);
                    VisibleVersion.Text = string.Concat("Minecraft ", OfflineVersionsList.SelectedItem.ToString());
                    
                    if(luco.HiddenWindow)
                        this.Hide();

                    la.Exited += (o, i) =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            VisibleWindow();
                        });
                    };

                    //mainwindow.Visibility = Visibility.Visible;

                    


                    qzqd = false;
                    qzqds.IsChecked = false;
                    OfflineQDTEXT.Text = "启动";

                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Wrong", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
            }
        }
        
        private void VisibleWindow()
        {
            mainwindow.Topmost = true;
            mainwindow.Show();
            mainwindow.Topmost = false;

        }

        private string getJavaWithVersion(int Javaversion)
        {

            if (Javaversion == 17)
            {
                return @"java17\bin\javaw.exe";
            }
            else
            {
                return @"java8\bin\javaw.exe";
            }
        }

        private void OfflineUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            string username = OfflineUsername.Text;
            char[] cUsername = username.ToCharArray();

            for (int i = 0; i < cUsername.Length; i++)
            {
                if (!((cUsername[i] >= 'a' && cUsername[i] <= 'z')
                    || (cUsername[i] >= 'A' && cUsername[i] <= 'Z')
                    || (cUsername[i] >= '0' && cUsername[i] <= '9')
                    || cUsername[i] == '_' || cUsername[i] == ' '))
                {
                    isUsernameOK = false;
                    isUserNameOKVisible.Visibility = Visibility.Visible;
                    qzqds.Visibility = Visibility.Visible;
                    qzqds.IsChecked = false;
                    return;
                }
            }

            int space = 0;

            for (int i = 0; i < cUsername.Length; i++)
            {
                if (username[i] == ' ')
                {
                    space++;
                }
            }

            if (!String.IsNullOrWhiteSpace(username))
            {
                isUsernameOK = true;
                isUserNameOKVisible.Visibility = Visibility.Hidden;
                qzqds.Visibility = Visibility.Hidden;
            }
            else
            {
                isUsernameOK = false;
                isUserNameOKVisible.Visibility = Visibility.Visible;
                qzqds.Visibility = Visibility.Visible;
                qzqds.IsChecked = false;
                return;
            }

            //config
            luco.username = OfflineUsername.Text;
            string js = JsonSerializer.Serialize(luco);
            
            writeConfigFile(js);

        }

        private void OfflineVersionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VisibleVersion.Text = string.Concat("Minecraft ", OfflineVersionsList.SelectedItem.ToString());
            
            //config
            luco.version = OfflineVersionsList.SelectedItem.ToString();
            string js = JsonSerializer.Serialize(luco);
            
            writeConfigFile(js);

            OnlineVersionsList.Text = OfflineVersionsList.SelectedItem.ToString();
            
            if (!String.IsNullOrEmpty(OfflineVersionsList.Text))
            {
                string filepath = $"ArrowLauncher\\GameConfigs\\{OfflineVersionsList.Text}.json";
                string jf = File.ReadAllText(filepath);
                GameConfig gc_ = JsonSerializer.Deserialize<GameConfig>(jf, new JsonSerializerOptions());
                VersionSulotion.IsChecked = gc_.IsVersionIsolation;
                
                if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                        , new JsonSerializerOptions()).JavaVersion == 0)
                    CustomJava.Text = "Auto";
                if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                        , new JsonSerializerOptions()).JavaVersion == 8)
                    CustomJava.Text = "Java8";
                if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                        , new JsonSerializerOptions()).JavaVersion == 17)
                    CustomJava.Text = "Java17";
            }
            
            
            

        }
        
        
        private void OnlineVersionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VisibleVersion.Text = string.Concat("Minecraft ", OnlineVersionsList.SelectedItem.ToString());
            OnlineVisibleVersion.Text = string.Concat("Minecraft ", OnlineVersionsList.SelectedItem.ToString());

            
            //config
            luco.version = OnlineVersionsList.SelectedItem.ToString();
            string js = JsonSerializer.Serialize(luco);
            
            writeConfigFile(js);

            OfflineVersionsList.Text = OnlineVersionsList.SelectedItem.ToString();
            
            
            
            if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                    , new JsonSerializerOptions()).JavaVersion == 0)
                CustomJava.Text = "Auto";
            if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                    , new JsonSerializerOptions()).JavaVersion == 8)
                CustomJava.Text = "Java8";
            if (JsonSerializer.Deserialize<GameConfig>(File.ReadAllText($"ArrowLauncher\\GameConfigs\\{luco.version}.json")
                    , new JsonSerializerOptions()).JavaVersion == 17)
                CustomJava.Text = "Java17";
        }

        private async void OnlineLaunchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(luco.Name))
                {
                    TargetGC = JsonSerializer.Deserialize<GameConfig>(
                    File.ReadAllText($"ArrowLauncher\\GameConfigs\\{OfflineVersionsList.Text}.json"));
                
                var lc = new LaunchConfig()
                {
                    Account = new Account
                    {
                        BaseAccount = new BaseAccount()
                        {
                            ClientToken = luco.ClientToken,
                            AccessToken = luco.AccessToken,
                            Name = luco.Name,
                            Uuid = luco.Uuid
                        }
                    },

                    JavaConfig = new JavaConfig
                    {
                        JavaPath = getJavaPathFromCustomJava(TargetGC.JavaVersion), //JavaPath
                        MaxMemory = 6198,
                        MinMemory = 1024
                    },

                    GameCoreConfig = new GameCoreConfig
                    {
                        Root = ".minecraft",
                        IsVersionIsolation = TargetGC.IsVersionIsolation,
                        Version = OnlineVersionsList.Text //版本
                    },

                        


                };
                
                MinecraftLauncher ml = new MinecraftLauncher(lc);
                var la = await ml.LaunchAsync(offlineReporter);

                VisibleVersion.Text = string.Concat("Minecraft ", OfflineVersionsList.SelectedItem.ToString());
                VisibleVersion.Text = $"启动游戏中 (80 %)";

                //await WaitForWindowWithTitleAsync(la.Process.Id, ml.JavaConfig.MinMemory);
                await Task.Delay(1000);
                //给点延迟qwq
                
                if(luco.HiddenWindow)
                    this.Hide();

                la.Exited += (o, i) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        VisibleWindow();
                    });
                };
                    
                VisibleVersion.Text = $"启动完成 (100 %)";
                await Task.Delay(1000);
                VisibleVersion.Text = string.Concat("Minecraft ", OfflineVersionsList.SelectedItem.ToString());
                OfflineQDTEXT.Text = "启动";   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Wrong");
            }
        }

        private string getJavaPathFromCustomJava(int a)
        {
            if (a == 0)
                return getJavaWithVersion(
                    GameCoreUtil.GetGameCore(
                        OnlineVersionsList.Text).JavaVersion);
            else if (a == 8)
            {
                return getJavaWithVersion(8);
            }
            else if (a == 17)
            {
                return getJavaWithVersion(17);
            }

            return getJavaWithVersion(
                GameCoreUtil.GetGameCore(
                    OnlineVersionsList.Text).JavaVersion);
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            qzqd = true;
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            qzqd = false;

        }

        private async void OnlineLoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            var auth = new MicrosoftAuthentication(ClientId);
            var deviceCodeInfo = await auth.RetrieveDeviceCodeInfo();
            
            Console.WriteLine(deviceCodeInfo.UserCode +  " " + deviceCodeInfo.VerificationUri);
            
            try
            {
                // 使用Process.Start方法，"url"作为要打开的网页的参数
                Clipboard.SetText(deviceCodeInfo.UserCode);
                await Task.Run(() =>
                {
                    var result = MessageBox.Show("代码已经复制到剪贴板", "提示");
                });  
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = deviceCodeInfo.VerificationUri,
                    UseShellExecute = true 
                });


            }
            catch (Exception ex)
            {
                Console.WriteLine("无法打开网页: " + ex.Message);
            }

            var TokenInfo = await auth.GetTokenResponse(deviceCodeInfo);
            var UserInfo = await auth.MicrosoftAuthAsync(TokenInfo, x =>
            {
                loginStatus.Text = x;
            });
            
            // Console.WriteLine(UserInfo.ClientToken);
            // Console.WriteLine(UserInfo.AccessToken);
            // Console.WriteLine(UserInfo.RefreshToken);
            // Console.WriteLine(UserInfo.DateTime);
            // Console.WriteLine(UserInfo.SkinUrl);
            // Console.WriteLine(UserInfo.Name);
            // Console.WriteLine(UserInfo.Uuid);

            // OA.AccessToken = UserInfo.AccessToken;
            // OA.ClientToken = UserInfo.ClientToken;
            // OA.Uuid = UserInfo.Uuid;
            // OA.Name = UserInfo.Name;
            luco.AccessToken = UserInfo.AccessToken;
            luco.ClientToken = UserInfo.ClientToken;
            luco.Uuid = UserInfo.Uuid;
            luco.Name = UserInfo.Name;
            
            TONULL.Visibility = Visibility.Visible;
            
            
            
            
            string js = JsonSerializer.Serialize(luco);
            writeConfigFile(js);

            OnlineLoginButton.Content = "重新登录";
        }



        private async void TONULL_OnClick(object sender, RoutedEventArgs e)
        {
            luco.AccessToken = null;
            luco.ClientToken = null;
            luco.Uuid = null;
            luco.Name = null;
            
            string js = JsonSerializer.Serialize(luco);
            writeConfigFile(js);
            
            OnlineLoginButton.Content = "登录账号";
            TONULL.Visibility = Visibility.Hidden;
            loginStatus.Text = "账号清空成功";
            await Task.Delay(1000);
            loginStatus.Text = "登录状态: ";
            

        }


        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(OfflineVersionsList.Text))
            {
                string filepath = $"ArrowLauncher\\GameConfigs\\{OfflineVersionsList.Text}.json";
                string jf = File.ReadAllText(filepath);
                GameConfig gc_ = JsonSerializer.Deserialize<GameConfig>(jf, new JsonSerializerOptions());
                gc_.IsVersionIsolation = true;
                jf = JsonSerializer.Serialize(gc_);
                File.WriteAllText(filepath, jf);
            }
            
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(OfflineVersionsList.Text))
            {
                string filepath = $"ArrowLauncher\\GameConfigs\\{OfflineVersionsList.Text}.json";
                string jf = File.ReadAllText(filepath);
                GameConfig gc_ = JsonSerializer.Deserialize<GameConfig>(jf, new JsonSerializerOptions());
                gc_.IsVersionIsolation = false;
                jf = JsonSerializer.Serialize(gc_);
                File.WriteAllText(filepath, jf);
            }
        }

        private void EnableCustomJava_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filepath = $"ArrowLauncher\\GameConfigs\\{OfflineVersionsList.Text}.json";
            string jf = File.ReadAllText(filepath);
            GameConfig gc_ = JsonSerializer.Deserialize<GameConfig>(jf, new JsonSerializerOptions());
            gc_.JavaVersion = CustomJavaTextToInt(CustomJava.SelectedItem.ToString());
            jf = JsonSerializer.Serialize(gc_);
            File.WriteAllText(filepath, jf);
        }

        private void HideWindowchecked(object sender, RoutedEventArgs e)
        {
            luco.HiddenWindow = true;
            string js = JsonSerializer.Serialize(luco);
            writeConfigFile(js);
        }

        private void HideWindowUnchecked(object sender, RoutedEventArgs e)
        {
            luco.HiddenWindow = false;
            string js = JsonSerializer.Serialize(luco);
            writeConfigFile(js);
        }

        private bool enabledGameName; //only V Disable
        private string _GameNameTemp;

        private async void GameInstallList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!InstallGameName.Equals(GameInstallList.Text))
                _GameNameTemp = InstallGameName.Text;
            
            if(!String.IsNullOrWhiteSpace(GameInstallList.Text))
                _GameNameTemp = GameInstallList.SelectedItem.ToString();
            
            InstallGameName.Text = GameInstallList.SelectedItem.ToString();
            enabledGameName = false;
            InstallGameName.IsReadOnly = true;
            InstallGameName.Background = Brushes.Gray;

            try
            {

                var a = await ForgeInstaller.EnumerableFromVersionAsync(GameInstallList.SelectedItem.ToString());
                ForgeInstallList.Items.Clear();
                foreach (var _a in a)
                {
                    ForgeInstallList.Items.Add(_a.ForgeVersion);
                    
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // try
            // {
            //     bool isgo = true;
            //     var vs = GameCoreUtil.GetGameCores(@".minecraft").ToArray();
            //     foreach (var v in vs)
            //     {
            //         if (InstallGameName.Text == v.Id)
            //         {
            //             CHONGMING.Visibility = Visibility.Visible;
            //             isgo = false;
            //         }
            //     }
            //
            //     if (!String.IsNullOrWhiteSpace(InstallGameName.Text) && isgo)
            //     {
            //         CHONGMING.Visibility = System.Windows.Visibility.Hidden;
            //         DownloadAPIs.SwitchDownloadSource(DownloadSource.BmclApi);
            //         MinecraftInstaller mi = new MinecraftInstaller(GameInstallList.SelectedItem.ToString());
            //
            //         mi.OnSpeedChanged += s =>
            //         {
            //             mainwindow.Dispatcher.Invoke(() => { InstallMessage2.Text = $"速度: {s} /s"; });
            //         };
            //         mi.OnProgressChanged += (s1, i) =>
            //         {
            //             mainwindow.Dispatcher.Invoke(() => { InstallMessage.Text = $"进度: {i}%"; });
            //         };
            //
            //         CancellationTokenSource cts = new CancellationTokenSource();
            //         CancellationToken ct = cts.Token;
            //
            //         await mi.InstallAsync(InstallGameName.Text, true, ct);
            //         InstallMessage.Text = $"进度: 100%";
            //
            //
            //         //MessageBox.Show("安装完成", "提示");
            //         Console.WriteLine("OK");
            //         vs = GameCoreUtil.GetGameCores(@".minecraft").ToArray();
            //         OfflineVersionsList.Items.Clear();
            //         OnlineVersionsList.Items.Clear();
            //
            //
            //         foreach (var v in vs)
            //         {
            //
            //             OfflineVersionsList.Items.Add(v.Id);
            //             OnlineVersionsList.Items.Add(v.Id);
            //         }
            //
            //     }
            // }
            // catch(OperationCanceledException)
            // {
            //     Console.WriteLine("Process Excited");
            // } //StarLight.Core此段代码废弃, 所以使用MinecraftLaunch

            if (String.IsNullOrEmpty(ForgeInstallList.SelectedItem.ToString()) &&
                !String.IsNullOrEmpty(GameInstallList.SelectedItem.ToString())) //fabric(if)
            {
                var resolver = new GameResolver(".minecraft");
                var vanlliaInstaller = new VanlliaInstaller(resolver, GameInstallList.SelectedItem.ToString(), MirrorDownloadManager.Bmcl);
                
                vanlliaInstaller.ProgressChanged += (_, x) => {
                    Console.WriteLine(x.ProgressStatus);
                    mainwindow.Dispatcher.Invoke(() => { InstallMessage.Text = $"进度: {x.Progress * 100}%"; });
                };
    
                var result = await vanlliaInstaller.InstallAsync();
                
    
                if (result) {
                    Console.WriteLine($"游戏核心安装成功"); 
                    Console.WriteLine("Download OK(doge");
                     
                     var vs = GameCoreUtil.GetGameCores(@".minecraft").ToArray();
                     OfflineVersionsList.Items.Clear();
                     OnlineVersionsList.Items.Clear();
                 
                 
                     foreach (var v in vs)
                     {
                 
                         OfflineVersionsList.Items.Add(v.Id);
                         OnlineVersionsList.Items.Add(v.Id);
                     }
                }
            }
            else if(!String.IsNullOrEmpty(GameInstallList.SelectedItem.ToString())
                    && !String.IsNullOrEmpty(ForgeInstallList.SelectedItem.ToString()))
            {
                var fbs = (await ForgeInstaller.EnumerableFromVersionAsync(GameInstallList.SelectedItem.ToString()));
                var forgebuild =
                    new ForgeInstallEntry();
                foreach (var fb in fbs)
                {
                    if (fb.ForgeVersion.Equals(ForgeInstallList.SelectedItem.ToString()))
                    {
                        forgebuild = fb;
                    }
                }
                    
                // ForgeInstaller installer = new ForgeInstaller(".minecraft", forgebuild, "Your JavaPath");
                //
                // installer.ProgressChanged += (_, x) => {
                //     Console.WriteLine(x.ProgressStatus);
                //     mainwindow.Dispatcher.Invoke(() => { InstallMessage.Text = $"进度: {x.Progress * 100}%"; });
                // };
                //
                // var result = await installer.InstallAsync();
                //
                // if (result) {
                //     Console.WriteLine($"游戏核心 {res.GameCore.Id} 安装成功");
                // }
            }

            
            
            //MessageBox.Show("安装完成", "提示");
            
            
             
             
             
        }

        private async void ForgeInstallList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            enabledGameName = true;
            InstallGameName.IsReadOnly = false;
            InstallGameName.Background = Brushes.White;

            if (!String.IsNullOrWhiteSpace(_GameNameTemp))
            {
                InstallGameName.Text = _GameNameTemp;
            } else
            
            InstallGameName.Text = $"{GameInstallList.SelectedItem.ToString()} Forge {ForgeInstallList.SelectedItem.ToString()}";
        }
    }
}