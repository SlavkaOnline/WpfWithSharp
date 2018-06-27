using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain.ModBus;
using ReactiveUI;
using MessageBox = System.Windows.MessageBox;

namespace WPFwithFsharp
{
    public class MainViewModel : ReactiveObject
    {
        public ReactiveCommand ConnectCommand { get; private set; }
        public ReactiveCommand DisconnectCommand { get; private set; }
        public ReactiveCommand WriteCommand { get; private set; }
        public ReactiveCommand ReadCommand { get; private set; }


        private Modbus.Client _client;

        public MainViewModel()
        {
            ConnectCommand = ReactiveCommand.CreateFromTask(() => Connect());
            DisconnectCommand = ReactiveCommand.CreateFromTask(() => Disconnect());
            WriteCommand = ReactiveCommand.Create(() => Write());
            ReadCommand = ReactiveCommand.Create(() => Read());
            
            _client = Modbus.CreateClient("127.0.0.1", 502);
        }

        private void Read()
        {
            var qry = Modbus.Queries.NewReadHoldingRegisters(4107, 10);
            var res = Modbus.QueryHandler(_client, qry);
            if (res.IsOk)
            {
                var regs = res.ResultValue;
                var str = regs.Aggregate(string.Empty, (current, reg) => current + $"{reg} ");
                MessageBox.Show(str);
            }
            else
            {
                MessageBox.Show(res.ErrorValue);
            }
        }

        private void Write()
        {
            var cmd = Modbus.Commands.NewWriteHoldingRegisters(4107, new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
            var res = Modbus.CommandHandler(_client, cmd);
            if (res.IsError)
            {
                MessageBox.Show(res.ErrorValue);
            }
        }


        public async Task Connect()
        {
            
            var res = Modbus.Connect(_client);
            if (res.IsError)
            {
                MessageBox.Show(res.ErrorValue);
            }
            else
            {
                _client = res.ResultValue;
                MessageBox.Show("Connected");
            }

            await Task.CompletedTask;
        }

        private Task Disconnect()
        {
            _client = Modbus.Disconnect(_client);
            return Task.CompletedTask;
        }
    }
}