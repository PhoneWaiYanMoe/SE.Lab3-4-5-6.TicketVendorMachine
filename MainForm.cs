using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketVendorMachine
{
    public partial class MainForm : Form
    {
        private string connectionString;

        public MainForm()
        {
            InitializeComponent();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TicketingSystemDB"].ConnectionString;
            LoadDestinations();
            LoadPaymentMethods();
        }
        private void LoadDestinations()
        {
            comboBoxDestination.Items.Add("Station A - $10");
            comboBoxDestination.Items.Add("Station B - $15");
            comboBoxDestination.Items.Add("Station C - $20");
        }

        private void LoadPaymentMethods()
        {
            comboBoxPaymentMethod.Items.Add("Credit Card");
            comboBoxPaymentMethod.Items.Add("Digital Wallet");
        }


        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void textBoxSeatQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalPrice();
        }

        private void comboBoxDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateTotalPrice();
        }
        private void CalculateTotalPrice()
        {
            if (comboBoxDestination.SelectedIndex == -1 || string.IsNullOrEmpty(textBoxSeatQuantity.Text))
            {
                labelTotalPrice.Text = "Total Price: $0.00";
                return;
            }

            string selectedDestination = comboBoxDestination.SelectedItem.ToString();
            decimal price = GetPriceFromDestination(selectedDestination);
            int seatQuantity;

            if (int.TryParse(textBoxSeatQuantity.Text, out seatQuantity))
            {
                decimal totalPrice = price * seatQuantity;
                labelTotalPrice.Text = $"Total Price: ${totalPrice:F2}";
            }
            else
            {
                labelTotalPrice.Text = "Total Price: $0.00";
            }
        }

        private decimal GetPriceFromDestination(string destination)
        {
            string[] parts = destination.Split('-');
            if (parts.Length > 1 && decimal.TryParse(parts[1].Trim().Replace("$", ""), out decimal price))
            {
                return price;
            }
            return 0;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (comboBoxDestination.SelectedIndex == -1 || string.IsNullOrEmpty(textBoxSeatQuantity.Text) || comboBoxPaymentMethod.SelectedIndex == -1)
            {
                labelStatus.Text = "Please complete all selections.";
                return;
            }

            string destination = comboBoxDestination.SelectedItem.ToString();
            int seatQuantity = int.Parse(textBoxSeatQuantity.Text);
            string paymentMethod = comboBoxPaymentMethod.SelectedItem.ToString();
            decimal price = GetPriceFromDestination(destination);
            decimal totalPrice = price * seatQuantity;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Tickets (Destination, PaymentMethod, SeatQuantity, TotalPrice) VALUES (@Destination, @PaymentMethod, @SeatQuantity, @TotalPrice)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Destination", destination);
                    cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@SeatQuantity", seatQuantity);
                    cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        labelStatus.Text = "Ticket purchased successfully!";
                    }
                    catch (Exception ex)
                    {
                        labelStatus.Text = "Error: " + ex.Message;
                    }
                }
            }

        }
    }
}
