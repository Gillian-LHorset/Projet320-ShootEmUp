namespace Scramble
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Cr�ation de la flotte de ships
            Ship ship = new Ship(0, AirSpace.HEIGHT / 2);

            // D�marrage
            Application.Run(new AirSpace(ship));
        }
    }
}