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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CA3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public enum CarSize { All, Small, Medium, Large };

    

    public partial class MainWindow : Window
    {
        //creating the link to db
        CA_S00190945Entities db = new CA_S00190945Entities();

        public int CarId { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            //load the collection for the size dropdown
            cbxSize.ItemsSource = Enum.GetNames(typeof(CarSize));

            //set value for CarId - will be used to see if car was selected or not
            CarId = -1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting the datepicker available only for future dates
            dpkStart.DisplayDateStart = DateTime.Now;
            dpkEnd.DisplayDateStart = DateTime.Now;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {            
            string selectedValue="";
            //check is size was selected
            if (cbxSize.SelectedIndex != -1)
            {
                //pick up the size selection
                selectedValue = cbxSize.SelectedItem.ToString();

                //initialize startDate and endDate
                DateTime startDate = DateTime.Now.Date.AddDays(-1);
                DateTime endDate = DateTime.Now.Date.AddDays(-1);
                //try - catch block to get the datepicker values and avoid no values
                try
                {
                    startDate = DateTime.Parse(dpkStart.Text);
                    endDate = DateTime.Parse(dpkEnd.Text);
                }
                catch
                {

                }
                if (startDate < endDate) // a valid date selection, start has to be before return date
                {
                    //select the cars in the selected size
                    var query = (from c in db.Cars
                                where c.Size == selectedValue
                                select c).ToList();

                    if (selectedValue == "All")
                    {
                        //select all the cars, all sizes
                        query = (from c in db.Cars
                                 select c).ToList();
                    }
                    //select the cars that are not available for the required period
                    var query2 = (from b in db.Bookings
                                  join c in db.Cars on b.CarId equals c.Id
                                  where (((startDate >= b.StartDate) && (endDate <= b.EndDate)) ||
                                            ((startDate < b.StartDate) && (endDate > b.StartDate)) ||
                                            ((startDate < b.EndDate) && (endDate > b.EndDate)))
                                  select c).ToList();
             
                    //substract from the availability list the cars that are allready booked
                    for(int i=0; i<query.Count; i++)
                    {
                        foreach(var q2 in query2)
                            if (query[i].Id == q2.Id )
                            {
                                query.Remove(query[i]);
                            }
                        if (query.Count == 0) MessageBox.Show("All cars booked for the selected dates and size!");
                    }
                    //load the car list                                
                    lbxCars.ItemsSource = query.ToList();
                }
                else
                {
                    //invalid date selection 
                    MessageBox.Show("Start date has to be before end date!");
                }

            }
            else
                //size was not selected....info to user
                MessageBox.Show("Please select car size!");
        }

        private void LbxCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //selecting a car from the availability list
            if (lbxCars.SelectedIndex != -1)
            {
                //car selected, getting the name and model                
                tbxSelected.Text=lbxCars.SelectedItem +"\nRendal Date:"+ dpkStart.Text+"\nReturn Date:" + dpkEnd.Text;   
                //set CarId to be availabal when saving the booking
                CarId = (int)lbxCars.SelectedValue;
            }     
            
        }

        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            //check if car was selected, CarId is set only after valid selection
            if (CarId != -1)
            {
                //get the dates
                DateTime startDate = (DateTime)dpkStart.SelectedDate;
                DateTime endDate = (DateTime)dpkEnd.SelectedDate;
                //create booking item with dates and carid
                Booking booking = new Booking()
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    CarId = CarId
                };
                //add booking to db and save
                db.Bookings.Add(booking);
                db.SaveChanges();
                //prep the details for confirm message
                var query = from c in db.Cars
                            where c.Id == CarId
                            select c;
                Car car = new Car();
                car = query.FirstOrDefault();
                MessageBox.Show("Booking Confirmation \n\n" +car +"\nStart Date:"+ startDate.ToString("dd/MM/yyyy") +
                                "\nReturn Date: " + endDate.Date.ToString("dd/MM/yyyy"));
                //clearing selection
                ResetSelection();
            }
            else
                //in case booking is pressed without vaid selection
                MessageBox.Show("Select an available car!");
            

        }

        private void DpkStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //if selection change reset available car list
            ResetSelection();
        }

        private void DpkEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //if selection change reset available car list
            ResetSelection();
        }

        private void CbxSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if selection change reset available car list
            ResetSelection();
        }

        public void ResetSelection()
        {
            //reset the available cars and selected car 
            lbxCars.ItemsSource = null;
            tbxSelected.Text = null;
        }

        #region stuff for testing 
        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            //display the bookings from the db - hidden on start - right click on car photo to activate
            Window1 secondWindow = new Window1();
            secondWindow.Owner = this;
            secondWindow.Show();
            var query = from b in db.Bookings
                        select b;
            secondWindow.dgBookings.ItemsSource = query.ToList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //THIS IS A TEST THIS IS A TEST THIS IS A TEST - access hidden on start up - - right click on car photo to activate
            string selectedValue = "";
            if (cbxSize.SelectedIndex != -1)
            {
                selectedValue = cbxSize.SelectedItem.ToString();
                DateTime startDate = DateTime.Now.Date.AddDays(-1);
                DateTime endDate = DateTime.Now.Date.AddDays(-1);
                try
                {
                    startDate = DateTime.Parse(dpkStart.Text);
                    endDate = DateTime.Parse(dpkEnd.Text);
                }
                catch
                {

                }
                if (startDate < endDate)
                {
                    var query = (from c in db.Cars
                                 where c.Size == selectedValue
                                 select c.Make + "-" + c.Model);

                    if (selectedValue == "All")
                    {
                        query = (from c in db.Cars
                                 select c.Make + "-" + c.Model);
                    }

                    var query2 = (from b in db.Bookings
                                  join c in db.Cars on b.CarId equals c.Id
                                  where (b.StartDate >= startDate) && (b.EndDate <= endDate) && (c.Size == selectedValue)
                                  select new 
                                  {
                                      Id = c.Id,
                                      Make = c.Make,
                                      Model = c.Model,
                                      Size = c.Size
                                  }).ToList();

                    //var query3 = from q1 in query
                    //             join q2 in query2 on q1.Id equals q2.Id into ps
                    //             from q2 in ps.DefaultIfEmpty()
                    //             select q1;


                    lbxCars.ItemsSource = query2.ToList();
                }
                else
                {
                    MessageBox.Show("Start date has to be before end date!");
                }

            }
            else
                MessageBox.Show("Please select car size!");

            
        }

        private void ImgCar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //making visible/hidden the 2 butons used for test purposes - right click on photo
            if (btnView.IsVisible)
            {
                btnView.Visibility = Visibility.Hidden;
                btnTaken.Visibility = Visibility.Hidden;
            }
            else
            {
                btnView.Visibility = Visibility.Visible;
                btnTaken.Visibility = Visibility.Visible;
            }     

            
        }
        #endregion
    }
}
