namespace ADATO.MVVM.Views;

public partial class Holidays : ContentPage
{
    List<string> holiDays = new List<string>();

    public Holidays()
	{
		InitializeComponent();
	}

    #region Wyswietlanie danych

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        EndSpeechBack();
    }
    protected override async void OnAppearing()
    {
        int numOfTodayDay1 = datePickerNameDay.Date.DayOfYear;

        base.OnAppearing();
        await LoadMauiAsset();
        holidays.Text = holiDays[numOfTodayDay1 - 1];


    }
    private void datePickerNameDay_DateSelected(object sender, DateChangedEventArgs e)
    {
        LoadMauiAsset();
        int numOfTodayDay = datePickerNameDay.Date.DayOfYear;
        holidays.Text = holiDays[numOfTodayDay - 1];
    }


    async Task LoadMauiAsset()
    {
        holiDays.Clear();

        int numOfTodayDay2 = datePickerNameDay.Date.DayOfYear;
        int year = datePickerNameDay.Date.Year;

        if (DateTime.IsLeapYear(year))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("Holidays.txt");
            using var reader = new StreamReader(stream);

            while (reader.Peek() != -1)
            {
                holiDays.Add(reader.ReadLine());

            }

        }
        else
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("HolidaysWO29Feb.txt");
            using var reader = new StreamReader(stream);

            while (reader.Peek() != -1)
            {
                holiDays.Add(reader.ReadLine());

            }

        }


    }
    #endregion

    #region Obsluga przycisków
    private void Button_Clicked(object sender, EventArgs e)
    {
        EndSpeechBack();
        Navigation.PopAsync();
    }
    private void Button_Next(object sender, EventArgs e)
    {
        datePickerNameDay.Date = datePickerNameDay.Date.AddDays(1);
    }
    private void Button_Previous(object sender, EventArgs e)
    {
        datePickerNameDay.Date = datePickerNameDay.Date.AddDays(-1);
    }
    #endregion


    #region Udostepnianie



    private void Button_Share(object sender, EventArgs e)
    {
        PreviousDay.IsVisible = false;
        NextDay.IsVisible = false;
        TitleText.Text = "Another Day at The Office";

        ScreenShare();
        TitleText.Text = "";
        TitleText.TextColor = TitleText.TextColor;
        PreviousDay.IsVisible = true;
        NextDay.IsVisible = true;
    }


    public async void ScreenShare()
    {

        //VisibleValue();
        var result = await GridScreenShare.CaptureAsync();

        using MemoryStream memoryStream = new MemoryStream();

        await result.CopyToAsync(memoryStream);


        string fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, "screen.png");

        File.WriteAllBytes(fullPath, memoryStream.ToArray());



        await Share.Default.RequestAsync(new ShareFileRequest
        {
            File = new ShareFile(fullPath),


        });

        //VisibleValue();
    }
    #endregion


    #region Text na mowe

    CancellationTokenSource cts;
    private async void StartSpeech(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(holidays.Text))
        {
            cts = new CancellationTokenSource();

            PlayText.IsVisible = false;
            EndText.IsVisible = true;

            await TextToSpeech.Default.SpeakAsync(holidays.Text.ToString(), cancelToken: cts.Token);


            PlayText.IsVisible = true;
            EndText.IsVisible = false;
        }
    }

    private void EndSpeech(object sender, EventArgs e)
    {
        EndSpeechBack();
    }

    public void EndSpeechBack()
    {
        if (cts?.IsCancellationRequested ?? true)
            return;

        cts.Cancel();
    }
    #endregion
}