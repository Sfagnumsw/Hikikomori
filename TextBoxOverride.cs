using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DT1
{
    public class TextBoxWatermark : TextBox    //Класс для видимости подсказок на текстбоксе
    {
        private string watermark;
        public string WatermarkTextBox 
        { 
            get { return watermark; }
            set { watermark = value; SetWatermark(); }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)] //Берем метод из Dll, который отправляет сообщение
        public static extern Int32 SendMessage(IntPtr hWnd, int msg, int wp, string lp);
         private void SetWatermark()
         {
            if(this.IsHandleCreated && watermark != null) // если дескриптор есть и текст сообщения не пустой то ок
            {
                SendMessage(this.Handle, 0x1501, 0, watermark);
            }
         }

        protected override void OnHandleCreated(EventArgs e) // вызов события
        {
            base.OnHandleCreated(e);
            SetWatermark();
        }
    }
}
