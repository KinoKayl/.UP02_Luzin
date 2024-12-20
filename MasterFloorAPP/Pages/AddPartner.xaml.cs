using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MasterFloorAPP.Pages
{
    public partial class AddPartner : Page
    {
        // Регулярные выражения для проверки форматов
        private static readonly Regex FIOregex = new Regex(@"^[А-ЯЁ][а-яё]+$"); 
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"); 
        private static readonly Regex INNRegex = new Regex(@"^\d{10}$"); 
        private static readonly Regex QuantityRegex = new Regex(@"^\d+$");
        private static readonly Regex PhoneNumberRegex = new Regex(@"^\+?[1-9]\d{9,14}$"); 

        private readonly Entities db; 
        private readonly Partners currentPartner; 

        public AddPartner(Partners selectedPartner = null)
        {
            InitializeComponent();
            db = new Entities(); 
            LoadTypes(); 
            LoadProducts();
            LoadProductTypes();
            currentPartner = selectedPartner; 

            
            if (currentPartner != null)
            {
                NameOrg.Text = currentPartner.Name;
                EmailOrg.Text = currentPartner.Email;
                NumberOrg.Text = currentPartner.Number;
                DirectorOrg.Text = currentPartner.Director;
                AddressOrg.Text = currentPartner.Address;
                INNOrg.Text = currentPartner.INN;  
                RatingOrg.Text = currentPartner.Rating.ToString();

                if (currentPartner.Type != null)
                {
                    Type.SelectedValue = currentPartner.Type; 
                }

                var partnerProduct = db.Partner_products
                               .FirstOrDefault(pp => pp.Partner == currentPartner.ID);

                if (partnerProduct != null)
                {
                    
                    TovariOrg.SelectedValue = partnerProduct.Product;
                    QuantityOrg.Text = partnerProduct.Quantity.ToString();

                    
                    var product = db.Products.FirstOrDefault(p => p.ID == partnerProduct.Product);
                    if (product != null)
                    {
                        var productType = db.Product_type.FirstOrDefault(pt => pt.ID == product.Type);
                        if (productType != null)
                        {
                            TovariTypes.SelectedValue = productType.ID; 
                        }
                    }
                }
            }
        }

        // Метод для загрузки типов партнеров в комбобокс
        private void LoadTypes()
        {
            var types = db.Partner_type.ToList(); 
            Type.ItemsSource = types; 
            Type.DisplayMemberPath = "Type"; 
            Type.SelectedValuePath = "ID"; 
        }

        private void LoadProducts()
        {
            var products = db.Products.ToList();
            TovariOrg.ItemsSource = products;
            TovariOrg.DisplayMemberPath = "Name";
            TovariOrg.SelectedValuePath = "ID";
        }

        private void LoadProductTypes()
        {
            var productTypes = db.Product_type.ToList();
            TovariTypes.ItemsSource = productTypes;
            TovariTypes.DisplayMemberPath = "Type";
            TovariTypes.SelectedValuePath= "ID";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string NameOrganization = NameOrg.Text;
            string DirectorOrganization = DirectorOrg.Text;
            string EmailOrganization = EmailOrg.Text;
            string NumberOrganization = NumberOrg.Text;
            string INNOrganization = INNOrg.Text;
            string RatingOrganization = RatingOrg.Text;
            string AddressOrganization = AddressOrg.Text;

            int? selectedType = (int?)Type.SelectedValue;

            string Quantity = QuantityOrg.Text;
            int? selectedProductTypeID = (int?)TovariTypes.SelectedValue;
            int? selectedProductID = (int?)TovariOrg.SelectedValue;

            try
            {
                // Валидация данных
                if (!FIOValidation(DirectorOrganization))
                {
                    MessageBox.Show(
                        "ФИО директора должно содержать от 2 до 5 слов, начинаться с заглавной буквы и содержать только буквы кириллицы.",
                        "Ошибка: Некорректное ФИО",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!PhoneNumberRegex.IsMatch(NumberOrganization))
                {
                    MessageBox.Show(
                        "Номер телефона должен быть в международном формате, например: +1234567890.",
                        "Ошибка: Некорректный номер телефона",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!EmailRegex.IsMatch(EmailOrganization))
                {
                    MessageBox.Show(
                        "Электронная почта должна быть в формате example@domain.com.",
                        "Ошибка: Некорректная электронная почта",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!INNRegex.IsMatch(INNOrganization))
                {
                    MessageBox.Show(
                        "ИНН должен содержать ровно 10 цифр.",
                        "Ошибка: Некорректный ИНН",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(RatingOrganization, out int ratingValue) || ratingValue < 1 || ratingValue > 10)
                {
                    MessageBox.Show(
                        "Рейтинг должен быть числом от 1 до 10.",
                        "Ошибка: Некорректный рейтинг",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!QuantityRegex.IsMatch(Quantity))
                {
                    MessageBox.Show(
                        "Количество должно быть числовым значением.",
                        "Ошибка: Некорректное количество",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

               
                if (currentPartner != null) 
                {
                    var attachedPartner = db.Partners.Find(currentPartner.ID);
                    if (attachedPartner == null)
                    {
                        MessageBox.Show("Партнёр не найден в текущем контексте.");
                        return;
                    }


                    currentPartner.Name = NameOrganization;
                    currentPartner.Director = DirectorOrganization;
                    currentPartner.Email = EmailOrganization;
                    currentPartner.Number = NumberOrganization;
                    currentPartner.INN = INNOrganization;
                    currentPartner.Address = AddressOrganization;
                    currentPartner.Rating = ratingValue;
                    currentPartner.Type = (int)selectedType;

                    var partnerProduct = db.Partner_products.FirstOrDefault(pp => pp.Partner == attachedPartner.ID);

                    if (partnerProduct != null)
                    {
                        partnerProduct.Product = selectedProductID ?? partnerProduct.Product;
                        partnerProduct.Quantity = int.TryParse(Quantity, out int quantityValue) ? quantityValue : partnerProduct.Quantity;
                    }
                    else if (selectedProductID.HasValue && int.TryParse(Quantity, out int quantityValue))
                    {
                        db.Partner_products.Add(new Partner_products
                        {
                            Partner = attachedPartner.ID,
                            Product = selectedProductID.Value,
                            Quantity = quantityValue,
                            SaleDATE = DateTime.Now
                        });
                    }

                    db.SaveChanges();
                    MessageBox.Show("Данные партнёра успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else 
                {
                    var newPartner = new Partners
                    {
                        Type = (int)selectedType,
                        Name = NameOrganization,
                        Director = DirectorOrganization,
                        Email = EmailOrganization,
                        Number = NumberOrganization,
                        INN = INNOrganization,
                        Address = AddressOrganization,
                        Rating = ratingValue
                    };

                    db.Partners.Add(newPartner);
                    db.SaveChanges();

                    int Partner = newPartner.ID;

                    if (int.TryParse(Quantity, out int quantityValue) && selectedProductID.HasValue)
                    {
                        db.Partner_products.Add(new Partner_products
                        {
                            Partner = Partner,
                            Product = selectedProductID.Value,
                            Quantity = quantityValue,
                            SaleDATE = DateTime.Now
                        });
                        db.SaveChanges();

                        MessageBox.Show(
                            "Партнер и продукция успешно добавлены.",
                            "Успех",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Некорректное количество продукции или продукт не выбран.",
                            "Предупреждение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }

                NavigationService?.Navigate(new Discounts());
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Произошла ошибка: {ex.Message}\nПожалуйста, обратитесь к администратору, если проблема повторяется.",
                    "Критическая ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }



        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); 
            ClearFields();
        }

        // Валидация ФИО директора
        private bool FIOValidation(string FIO)
        {
            var splitFIO = FIO.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); 

            if (splitFIO.Length < 2 || splitFIO.Length > 5)
            {
                return false;
            }

            foreach (var part in splitFIO)
            {
                if (string.IsNullOrWhiteSpace(part) || !FIOregex.IsMatch(part) || part.Length > 50)
                {
                    return false;
                }
            }
            return true;
        }

        private void ClearFields()
        {
            Type.SelectedItem = null;
            NameOrg.Text = string.Empty;
            DirectorOrg.Text = string.Empty;
            AddressOrg.Text = string.Empty;
            NumberOrg.Text = string.Empty;
            INNOrg.Text = string.Empty;
            RatingOrg.Text = string.Empty;
            EmailOrg.Text = string.Empty;
            TovariTypes.SelectedItem = null;
            TovariOrg.SelectedItem = null;
            QuantityOrg.Text = string.Empty;
        }
    }
}