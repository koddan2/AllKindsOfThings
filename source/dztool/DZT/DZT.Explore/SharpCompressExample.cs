
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace DZT.Explore;

internal class SharpCompressExample
{
    internal static void Run()
    {
        var sourceDir = @"C:\Program Files (x86)\Steam\steamapps\common\DayZServer\.dzt";
        var outputDir = @"c:\temp\compress";
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        var compressedFile = Path.Combine(outputDir, "files.tgz");

        {
            using Stream stream = File.OpenWrite(compressedFile);
            var writerOptions = new WriterOptions(CompressionType.GZip)
            {
                LeaveStreamOpen = true,
            };
            using var writer = WriterFactory.Open(stream, ArchiveType.Tar, writerOptions);
            writer.WriteAll(sourceDir, "*", SearchOption.AllDirectories);
        }
        // --
        {
            using Stream stream = File.OpenRead(compressedFile);
            using var reader = ReaderFactory.Open(stream);
            var decompressedContainerNameFromFileName = Path.GetFileNameWithoutExtension(compressedFile);

            // name.tar.gz => name.tar => name
            if (Path.GetExtension(decompressedContainerNameFromFileName) is ".tar")
            {
                decompressedContainerNameFromFileName = Path.GetFileNameWithoutExtension(decompressedContainerNameFromFileName);
            }

            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    using (var entryStream = reader.OpenEntryStream())
                    {
                        var writeFilePath = Path.Combine(outputDir, decompressedContainerNameFromFileName, reader.Entry.Key);
                        var writeFileDir = Path.GetDirectoryName(writeFilePath);
                        if (!Directory.Exists(writeFileDir) && writeFileDir is string)
                        {
                            Directory.CreateDirectory(writeFileDir);
                        }
                        using Stream writeStream = File.Open(writeFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                        entryStream.CopyTo(writeStream);
                        writeStream.Flush();
                    }
                }
            }
        }
    }
}
