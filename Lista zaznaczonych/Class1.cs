using System;
using Hydra;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SqlClient;


[assembly: CallbackAssemblyDescription("Update Termin Realizacji Terminarz",
"Update Termin Realizacji Terminarz",
"Krzysztof Kurowski",
"1.0",
"2023.1",
"09-02-2024")]

namespace UpdateTerminRealizacjiTerminarz
{
    [SubscribeProcedure((Procedures)Procedures.ZadanieEdycja, "callback na zadaniu")]
    public class callbacktestowy : Callback
    {
        ClaWindow button;
        ClaWindow ButtonParent;
        private string ConnectionString { get; } = $"user id=Gaska;password=mNgHghY4fGhTRQw;Data Source=192.168.0.105;Trusted_Connection=no;database={Runtime.ActiveRuntime.Repository.Connection.Database};connection timeout=5;";

        public override void Init()
        {
            AddSubscription(true, 0, Events.OpenWindow, new TakeEventDelegate(OnOpenWindow)); // Otwarcie okna
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(ChangeWindow)); // zmiana szerokosci/wysokosci okna
        }

        public bool OnOpenWindow(Procedures ProcId, int ControlId, Events Event)
        {
            ClaWindow Parent = GetWindow();
            ButtonParent = Parent.AllChildren["?ListOkresPowtarzania"]; // od ktorego przycisku
            button = Parent.Children["?GroupTerminRealizacji"].Children.Add(ControlTypes.button); // w ktorej belce
            button.Visible = true;
            button.Bounds = new Rectangle(Convert.ToInt32(ButtonParent.XposRaw) + 84, Convert.ToInt32(ButtonParent.YposRaw) - 10, 112, 18);
            button.TextRaw = $"Zmień termin realizacji";


            AddSubscription(false, button.Id, Events.Accepted, new TakeEventDelegate(NewButton_OnAfterMouseDown));
            return true; 
        }

        public bool ChangeWindow(Procedures ProcId, int ControlId, Events Event)
        {
            button.Bounds = new Rectangle(Convert.ToInt32(ButtonParent.XposRaw) + 84, Convert.ToInt32(ButtonParent.YposRaw) - 10, 112, 18);
            return true;
        }

        public bool NewButton_OnAfterMouseDown(Procedures ProcedureId, int ControlId, Events Event)
        {
            try
            {
                TerminRealizacjiForm form1 = new TerminRealizacjiForm(Zadania.Zad_Id, ConnectionString);
                form1.Show();
            }
            catch (Exception ex)
            {
                Runtime.WindowController.UnlockThread();
                MessageBox.Show("Błąd:" + ex.Message);
                Runtime.WindowController.LockThread();
            }
            Runtime.WindowController.PostEvent(0, Events.FullRefresh);

            return true;
        }

        public override void Cleanup()
        {
        }
    }
}
