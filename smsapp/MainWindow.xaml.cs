using SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace smsapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SMSNet client;
        AddressBook addressbook;
        SMSAccount mke;
        public MainWindow()
        {
            InitializeComponent();

            addressbook = new AddressBook();

//            addressbook.AddContact("MRZRBgIAAAC\tAABSU0ExAAQAAAEAAQDdEHaTbjquPhR0KSWR9Jr2I+6QtkANZqQGu9lHkNQBocy7M0jRlTV+Jd3A/0/P+ixJUazLbV7BlXfHgwomTdX3g9FbnafjHM5psmX5ZDGN/YAKFUGkH+Gt+otxDq8K2wNlUMChSL3wpYqOrsFRSVzpXkk1Xmtgzd6BfXveewEIuA==");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            
            MORZEContact alice = new MORZEContact("Alice", "MRZRBgIAAACkAABSU0ExAAQAAAEAAQDdEHaTbjquPhR0KSWR9Jr2I+6QtkANZqQGu9lHfNQBocy7M0jRlTV+Jd3A/0/P+ixJUazLbV7BlXfHgwomTdX3g9FbnafjHM5psmX5ZDGN/YAKFUGkH+Gt+otxDq8K2wNlUMChSL3wpYqOrsFRSVzpXkk1Xmtgzd6BfXveewEIuA==");
            
            client.SendMessage("Hello Alice", alice);


           
           

           // hash=client.Send(sms);
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            button_Click(sender, e);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
             mke=new SMSAccount("YURIY");
            mke.AddressBook = addressbook;


            string err = mke.LoadKey(null);
            if (string.IsNullOrEmpty(err) == false)
            {
                mke.GenerateKey();
                err = mke.SaveKey(null);
            }
            if (string.IsNullOrEmpty(err) == false)
                MessageBox.Show(err);



            client = new SMSNet(mke);
            client.Connect("127.0.0.1", 5555);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
           mke = new SMSAccount("ALICE");
            mke.AddressBook = addressbook;
            string err = mke.LoadKey(null);
            if (string.IsNullOrEmpty(err) == false)
            {
                mke.GenerateKey();
                err = mke.SaveKey(null);
            }
            if (string.IsNullOrEmpty(err) == false)
                MessageBox.Show(err);



            client = new SMSNet(mke);
            client.Connect("127.0.0.1", 5555);
        }
    }
}
