using CommunityToolkit.Maui.Storage;
using System.Text;

namespace Lab
{
    class FileSystem
    {
        IFileSaver fileSaver;
        CancellationTokenSource cancellationTokenSource;

        BoardSerializer boardSerializer = new();

        public FileSystem(IFileSaver fileSaver)
        {
            this.fileSaver = fileSaver;
            cancellationTokenSource = new();
        }

        public async void Save(GridBoard board)
        {
            string content = boardSerializer.Serialize(board);

            using var stream = new MemoryStream(Encoding.Default.GetBytes(content));
            await fileSaver.SaveAsync("board.save", stream, cancellationTokenSource.Token);
        }

        public async Task<GridBoard?> LoadAsync()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                {DevicePlatform.Unknown, new[] {".save"} },
                {DevicePlatform.WinUI, new[] {".save"} },
                {DevicePlatform.iOS, new[] {".save"} },
                {DevicePlatform.Android, new[] {".save"} }
            });

            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Pick board",
                FileTypes = customFileType
            });

            if (result == null) return null;

            using var stream = await result.OpenReadAsync();
            using var reader = new StreamReader(stream);

            string content = await reader.ReadToEndAsync();
            GridBoard board = boardSerializer.DeSerialize(content);

            return board;
        }

    }
}
