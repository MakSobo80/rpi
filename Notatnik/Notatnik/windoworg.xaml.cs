using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Notatnik
{
    /// <summary>
    /// Logika interakcji dla klasy windoworg.xaml
    /// </summary>
    public partial class windoworg : Window
    {
        private class MemberItem
        {
            public byte Id { get; set; }
            public string DisplayText { get; set; } = string.Empty;
            public bool IsManager { get; set; }
        }

        public windoworg()
        {
            InitializeComponent();
            Loaded += WindowOrg_Loaded;
        }

        private void WindowOrg_Loaded(object sender, RoutedEventArgs e)
        {
            OdswiezWidok();
        }

        private void OdswiezWidok()
        {
            var user = Session.LoggedInUser;
            if (user == null || user.organizationId == null)
            {
                labelOrgName.Content = "Brak organizacji";
                return;
            }

            var org = Database.GetOrganization(user.organizationId);
            labelOrgName.Content = org?.Name?.Trim() ?? "—";

            var members = Database.GetUsersInOrganization(user.organizationId.Value);
            listMembers.Items.Clear();
            foreach (var m in members)
            {
                listMembers.Items.Add(new MemberItem
                {
                    Id = m.Id,
                    DisplayText = m.Username.Trim() + (m.IsManager ? " [Manager]" : ""),
                    IsManager = m.IsManager
                });
            }

            bool isManager = user.isManager == true;
            panelManagerButtons.Visibility = isManager ? Visibility.Visible : Visibility.Collapsed;
            panelNowOrg.Visibility = isManager ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UsunUzytkownika_Click(object sender, RoutedEventArgs e)
        {
            if (listMembers.SelectedItem is not MemberItem selected)
            {
                MessageBox.Show("Wybierz użytkownika z listy.", "Informacja");
                return;
            }

            var currentUser = Session.LoggedInUser;
            var dbCurrentUser = currentUser != null ? Database.GetUser(currentUser.Login!) : null;
            if (dbCurrentUser != null && dbCurrentUser.Id == selected.Id)
            {
                MessageBox.Show("Nie możesz usunąć samego siebie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć '{selected.DisplayText}' z organizacji?",
                "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Database.RemoveUserFromOrganization(selected.Id);
                OdswiezWidok();
            }
        }

        private void ZmienManagera_Click(object sender, RoutedEventArgs e)
        {
            if (listMembers.SelectedItem is not MemberItem selected)
            {
                MessageBox.Show("Wybierz użytkownika z listy.", "Informacja");
                return;
            }

            bool newStatus = !selected.IsManager;
            string action = newStatus ? "nadać uprawnienia managera" : "odebrać uprawnienia managera";
            var result = MessageBox.Show(
                $"Czy na pewno chcesz {action} użytkownikowi '{selected.DisplayText.Split(' ')[0]}'?",
                "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Database.SetUserManager(selected.Id, newStatus);
                OdswiezWidok();
            }
        }

        private void UtworzOrg_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNewOrgName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Podaj nazwę organizacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Database.AddOrganization(name);
            txtNewOrgName.Clear();
            MessageBox.Show($"Organizacja '{name}' została utworzona.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Powrot_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new WindowAdmin();
            adminWindow.Show();
            this.Close();
        }
    }
}
