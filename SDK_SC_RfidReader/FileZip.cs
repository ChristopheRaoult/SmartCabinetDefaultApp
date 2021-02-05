using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace SDK_SC_RfidReader
{
    /// <summary>
    /// Enumeration of possible action in ZIP
    /// </summary>
    public enum Action 
    {
        /// <summary>
        /// Value for Zip
        /// </summary>
        Zip, 
        /// <summary>
        /// Value for unzip
        /// </summary>
        UnZip 
    };
    class FileZip
    {
        /// <summary>
        /// This is a private field that represents
        /// the full source file path
        /// </summary>
        private string _SourceFileName ="";
        /// <summary>
        /// This is a private field that represents
        /// the full destination file path
        /// </summary>
        private string _DestinationFileName ="";
        /// <summary>
        /// This byte array is used to stock both
        /// The input file contents and out put file
        /// contents as bytes
        /// </summary>
        private byte[] oBuffer;
        /// <summary>
        /// This is the class responsible of
        /// zipping and unzipping files
        /// </summary>
        private GZipStream oZipper;
        /// <summary>
        /// This is a default constructor
        /// </summary>
        public FileZip() { }
        /// <summary>
        /// This is an overloaded constructor
        /// </summary>
        /// <param name="SourceFileName">This represents the
        /// full source file name of the one going to be zipped
        /// </param>
        /// <param name="DestinationFileName">This represents the
        /// full source file name of the one going to be unziped
        /// </param>
        /// <param name="action">Choose between zip or unzip mode</param>
        public FileZip(string SourceFileName, string DestinationFileName, Action action)
        {
            oZipper = null;
            this.SourceFileName = SourceFileName;
            this.DestinationFileName = DestinationFileName;
            /* The user only precizes the zip mode
            or the desired action in order to be performed
            * instead of using the method directly that is
            marked as protected, take a look below */
            if (action == Action.Zip)
            { this.CompressFile(); }
            if (action == Action.UnZip)
            { this.DecompressFile(); }
 
        }
        /// <summary>
        /// This is the source file full path property
        /// </summary>
        public string SourceFileName
        {
            get
            {
            return _SourceFileName;
            }
            set
            {
            _SourceFileName = value;
            }
        }
        /// <summary>
        /// This is the destination full path property
        /// </summary>
        public string DestinationFileName
        {
            get
            {
            return _DestinationFileName;
            }
            set
            {
            _DestinationFileName = value; 
            }
        }
        /// <summary>
        /// This is the method responsible for compression, it is marked
        /// as protected because we use it is called at the constructor
        /// level when a compression mode is chosen instead of using it directly
        /// </summary>
        protected void CompressFile()
        {

            if (File.Exists(SourceFileName))
            {
                /*   using (FileStream inputFile = File.Open(SourceFileName, FileMode.Open), outputFile = File.Create(DestinationFileName))
                {
                using (oZipper = new GZipStream(outputFile, CompressionMode.Compress))
                {
                    oBuffer = new byte[inputFile.Length];
                    int counter = 0;
                    while ((counter = inputFile.Read(oBuffer, 0, oBuffer.Length)) != 0)
                    {
                        oZipper.Write(oBuffer, 0, counter);
                    }
                }
                oBuffer = null;
                }
            }  */

                try
                {
                    // Le fichier est placé dans le FileStream
                    FileStream monFileStream = new FileStream(SourceFileName, FileMode.Open);

                    byte[] monBuffer = new byte[monFileStream.Length];
                    // Lecture de l'intégralité du FileStream
                    monFileStream.Read(monBuffer, 0, System.Convert.ToInt32(monFileStream.Length));
                    // Fermeture du FileStream
                    monFileStream.Close();
                    // Création du fichier qui va contenir le fichier compressé
                    monFileStream = new FileStream(DestinationFileName, FileMode.Create);

                    // Compression des données
                    GZipStream monGZipStream = new GZipStream(monFileStream, CompressionMode.Compress, false);
                    // Ecriture des données compressées dans le fichier de destination
                    monGZipStream.Write(monBuffer, 0, monBuffer.Length);
                    // Fermeture du GZipStream
                    monGZipStream.Close();
                    
                }
                catch 
                {
                    //throw new Exception();
                }
            }
 

        //TO DO here notify user that the task is performed
        }
        /// <summary>
        /// This is the method responsible for compression, it is marked
        /// as protected because we use it is called at the constructor
        /// level when a decompression mode is chosen instead of using it directly
        /// </summary>
        protected void DecompressFile()
        {
 
            if (File.Exists(SourceFileName))
            {
                using (FileStream inputFile = File.Open(SourceFileName, FileMode.Open), outputFile = File.Create(DestinationFileName))
                {
                using (oZipper = new GZipStream(inputFile, CompressionMode.Decompress))
                {
                oBuffer = new byte[inputFile.Length];
                int counter;
                    while ((counter = oZipper.Read(oBuffer, 0, oBuffer.Length)) != 0)
                    {
                        outputFile.Write(oBuffer, 0, counter);
                    }
                }
                oBuffer = null;
                }
            }
        
        }
    }
//TO DO here notify user that the task is performed
 
}
    
