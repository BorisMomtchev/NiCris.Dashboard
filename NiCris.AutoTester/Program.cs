using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NiCris.AutoTester.Messages;
using System.Text;

namespace NiCris.AutoTester
{
    class Tester
    {
        #region Win32 APIs ...
        
        private const uint STD_OUTPUT_HANDLE = 0xfffffff5;
        private static IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        #endregion

        #region Constants ...

        // Color schema
        private const int defaultGrey = 007;
        private const int green = 010;
        private const int lightBlue = 011;
        private const int red = 012;
        private const int white = 015;
        private const int exceptionSchema = 207;

        private const string appName = "Message Tester";

        //Return codes: 0(no failed messages); X (X is the number of failed messages)
        //In case of exception the app doesn't quit, so the user can see the easily.
        private static int returnCode = 0;
        private static bool batchRun = false;

        #endregion

        #region Main methods...

        // Entry point for the app
        static int Main(string[] args)
        {
            CustomConsoleWrite(appName + " started", lightBlue, false);
            string fileName = string.Empty;
            string cachedFileName = string.Empty;
            bool detailMode = true;

            if (args.Length > 0)
            {
                batchRun = true;
                fileName = args[0];
            }

            do
            {
                if (fileName == string.Empty)
                {
                    Console.WriteLine("\nSelect Menu:" +
                                        "\n 'Q' + ENTER to Quit" +
                                        "\n 'R' + ENTER to Repeat the Last XML Config File" +
                                        "\n 'D' + ENTER to Toggle the DETAIL_MODE" +
                                        "\n OR \n => Please type the Full Path to an XML Config File + ENTER to (Re)Configure");
                    CustomConsoleSetColor(lightBlue);
                    fileName = @Console.ReadLine().ToUpper();
                    CustomConsoleRestore();
                }
                else
                {
                    cachedFileName = fileName;
                    Tester tester = new Tester();
                    tester.Execute(fileName, detailMode);
                    fileName = string.Empty;
                }

                if (fileName == "R")
                    fileName = cachedFileName;
                if (fileName == "D")
                {
                    fileName = string.Empty; ;
                    detailMode = !detailMode;
                    Console.WriteLine("->DETAIL_MODE = " + detailMode.ToString().ToUpper());
                }

            } while ((!batchRun && fileName != "Q") || (batchRun && returnCode == -1 && fileName != "Q"));

            return returnCode;
        }

        // Process all messages as configured
        private void Execute(string fileName, bool detailMode)
        {
            try
            {
                returnCode = 0;     // Assume success

                Process process = Process.GetCurrentProcess();

                // Create our message factory
                MessageFactory.Configure(fileName);
                MessageFactory msgFactory = MessageFactory.Instance;

                if (MessageFactory.DynamicLibrary)
                {
                    Console.WriteLine("-> Compiling DynamicLibrary.dll from source...");
                    
                    Stopwatch watch = new Stopwatch(); 
                    watch.Start();
                    MessageFactory.CompileDynamicLibrary();
                    watch.Stop();

                    Console.WriteLine(string.Format("-> DynamicLibrary.dll created in {0} ms.", 
                        watch.ElapsedMilliseconds.ToString()));
                }

                Console.WriteLine("-> " + MessageFactory.NumOfScheduledMessages.ToString() + " message(s) Scheduled.");

                //Set the title, so the user can see the running config at all time...
                string windowTitle = "Service URI: " + MessageFactory.ServiceURI +
                                     ";  " + DateTime.Now.ToString() +
                                     ";  ESC to Cancel...";
                Console.Title = windowTitle;

                // Screen
                if (!batchRun)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press ENTER to SEND the message(s):");
                    Console.ReadLine();
                    Console.WriteLine("PLEASE WAIT...");
                }

                // Start sending & listen to the music of the Console
                for (int i = 1; i <= MessageFactory.NumOfScheduledMessages; i++)
                {
                    // Oh-oh ESC key has been pressed...
                    if (GetAsyncKeyState(0x1B) != 0)
                    {
                        // GetForegroundWindow is weird under Win7/Vista...
                        //if (hWnd == (IntPtr)GetForegroundWindow())
                        if (windowTitle == GetActiveWindowTitle())
                        {
                            CustomConsoleSetColor(red);
                            Console.WriteLine("DO YOU WANT TO CANCEL THIS TEST?");
                            Console.WriteLine("Press: 'y + ENTER' to Cancel or 'ENTER' to Continue:");

                            if (Console.ReadLine().ToUpper() == "Y")
                                throw new Exception("This test has been canceled.");
                            else
                                CustomConsoleRestore();
                        }
                    }

                    Console.Write("Processing Msg -" + i.ToString() + "- ");
                    if (detailMode)
                    {
                        Console.Write(" Failed so far: ");
                        int failedSoFar = MessageFactory.GetAllFailed();

                        if (failedSoFar > 0)
                            CustomConsoleWrite(failedSoFar.ToString(), red, false);
                        else
                            CustomConsoleWrite(failedSoFar.ToString(), green, false);
                    }

                    Console.Write(msgFactory.SendMessage(i, detailMode));

                    if (msgFactory.GetMessageByIndex(i).SuccessfullyCompleted)
                        CustomConsoleWrite("<SUCCESS>", green, false);
                    else
                        CustomConsoleWrite("<FAILURE>", red, false);

                    Console.WriteLine();
                    Console.WriteLine();
                }

                // We're done with this session - report
                Console.WriteLine("Processing Complete...");
                returnCode = MessageFactory.GetAllFailed(); // + MessageFactory.UnexpectedAckList.Count;
                Console.WriteLine("ReturnCode: " + returnCode);
                CustomConsoleWrite(MessageFactory.CreateReport(), lightBlue, false);
            }
            catch (Exception ex)
            {
                // In a case of exception - set the returnCode so the app won't quit & report
                returnCode = -1;
                if (MessageFactory.IsConfigured)
                {
                    try
                    {
                        Console.WriteLine();
                        CustomConsoleWrite(MessageFactory.CreateReport(), lightBlue, false);
                    }
                    catch (Exception inEx)
                    {
                        CustomConsoleSetColor(exceptionSchema);
                        Console.WriteLine("\n- EXCEPTION -");
                        Console.WriteLine("SOURCE: " + inEx.Source);
                        Console.WriteLine("MESSAGE: " + inEx.Message);
                        CustomConsoleRestore();
                    }
                }

                CustomConsoleSetColor(exceptionSchema);
                Console.WriteLine("\n- EXCEPTION -");
                Console.WriteLine("SOURCE: " + ex.Source);
                Console.WriteLine("MESSAGE: " + ex.Message);
                CustomConsoleRestore();
            }
        }

        // Helps us identify the active window...
        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder buff = new StringBuilder(nChars);
            handle = (IntPtr)GetForegroundWindow();

            if (GetWindowText(handle, buff, nChars) > 0)
            {
                return buff.ToString();
            }
            return null;
        }

        #endregion

        #region Custom style ...

        private static void CustomConsoleWrite(string stringToWrite, int colorCode, bool defaultConsole)
        {
            SetConsoleTextAttribute(hConsole, colorCode);
            Console.Write(stringToWrite);

            if (defaultConsole)
                SetConsoleTextAttribute(hConsole, defaultGrey);
            else
                SetConsoleTextAttribute(hConsole, white);
        }

        private static void CustomConsoleSetColor(int colorCode)
        {
            SetConsoleTextAttribute(hConsole, colorCode);
        }

        private static void CustomConsoleRestore()
        {
            SetConsoleTextAttribute(hConsole, white);
        }

        #endregion
    }
}
