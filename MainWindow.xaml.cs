using AutoLotModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Palacian_Ioana_Teodora_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing

    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;
        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();
        Binding colorTextBoxBinding = new Binding();
        Binding makeTextBoxBinding = new Binding();
        Binding cmbCustomersBinding = new Binding();
        Binding cmbInventoryBinding = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            colorTextBoxBinding.Path = new PropertyPath("Color");
            makeTextBoxBinding.Path = new PropertyPath("Make");
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
            cmbCustomersBinding.Path = new PropertyPath("CustId");
            cmbInventoryBinding.Path = new PropertyPath("CarId");
            cmbCustomers.SetBinding(ComboBox.SelectedValueProperty, cmbCustomersBinding);
            cmbInventory.SetBinding(ComboBox.SelectedValueProperty, cmbInventoryBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Inventories.Load();
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Orders.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if(action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else
                if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(customer);
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            }
            setValidatitonBinding();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnNext.IsEnabled = false;
            btnPrev.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            setValidatitonBinding();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            customerDataGrid.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    inventory = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }
            else
                if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(inventory);
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
            }
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnNext1.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            makeTextBox.Text = "";
        }

        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
        }

        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;
            inventoryDataGrid.IsEnabled = true;
            btnPrev1.IsEnabled = true;
            btnNext1.IsEnabled = true;
            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
        }

        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnNext2.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            ordersDataGrid.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;
            BindingOperations.ClearBinding(cmbCustomers, ComboBox.SelectedValueProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.SelectedValueProperty);


        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {

                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
            }
            else
                if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                    
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                customerOrdersViewSource.View.Refresh();
                customerOrdersViewSource.View.MoveCurrentTo(selectedOrder);
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
                cmbCustomers.SetBinding(ComboBox.SelectedValueProperty, cmbCustomersBinding);
                cmbInventory.SetBinding(ComboBox.SelectedValueProperty, cmbInventoryBinding);
            }
            else if (action == ActionState.Delete)
            {
                
                try
                {  

                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch(DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerOrdersViewSource.View.Refresh();
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbInventory.IsEnabled = false;
                cmbCustomers.SetBinding(ComboBox.SelectedValueProperty, cmbCustomersBinding);
                cmbInventory.SetBinding(ComboBox.SelectedValueProperty, cmbInventoryBinding);

            }
        }

        private void btnEdit2_click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempCustId = cmbCustomers.Text.ToString();
            string tempCarId = cmbInventory.Text.ToString();
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;
            BindingOperations.ClearBinding(cmbCustomers, ComboBox.SelectedValueProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.SelectedValueProperty);
            cmbCustomers.Text = tempCustId;
            cmbInventory.Text = tempCarId;
        }

        private void btnDelete2_click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempCustId = cmbCustomers.Text.ToString();
            string tempCarId = cmbInventory.Text.ToString();
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            BindingOperations.ClearBinding(cmbCustomers, ComboBox.SelectedValueProperty);
            BindingOperations.ClearBinding(makeTextBox, ComboBox.SelectedValueProperty);
            cmbCustomers.Text = tempCustId;
            cmbInventory.Text = tempCarId;
        }

        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;
            ordersDataGrid.IsEnabled = true;
            btnPrev2.IsEnabled = true;
            btnNext2.IsEnabled = true;
            cmbCustomers.IsEnabled = false;
            cmbInventory.IsEnabled = false;
            cmbCustomers.SetBinding(ComboBox.SelectedValueProperty, cmbCustomersBinding);
            cmbInventory.SetBinding(ComboBox.SelectedValueProperty, cmbInventoryBinding);
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnPrev2_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }
        private void setValidatitonBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty,
           firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty,
           lastNameValidationBinding); 
        }
    }
}
