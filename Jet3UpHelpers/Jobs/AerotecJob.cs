using Jet3UpHelpers;
using Microsoft.VisualBasic;

namespace Helpers.Jobs
{
    public enum Keys
    {
        HTZ,
        Signature,
        ANR,
        BTIDX,
        ControllerId,
        Anzahl
    }
    /// <summary>
    /// Specific implementation fo a job for Aerotec.
    /// </summary>
    public class AerotecJob : Job
    {
        public AerotecJob(string HTZ, string signature, string ANR, string BTIDX, string controllerId, string? anzahl = null) 
        {
            AerotecObjects.Add(Keys.HTZ, HTZ);
            AerotecObjects.Add(Keys.Signature, signature);
            AerotecObjects.Add(Keys.ANR, ANR);
            AerotecObjects.Add(Keys.BTIDX, BTIDX);
            AerotecObjects.Add(Keys.ControllerId, controllerId);
            if(anzahl!= null)
            {
                AerotecObjects.Add(Keys.Anzahl, anzahl);
            }
        }

        public MachineTypeEnum MachineType { get; set; } = MachineTypeEnum.Neagra;

        /*inheritdoc*/
        public int Delay { get; set; }

        /*inheritdoc*/
        public FontSizeEnum FontSize { get; set; }

        /*inheritdoc*/
        public int Rotation { get; set; }

        /*inheritdoc*/
        public int EncoderResolution { get; set; }

        /*inheritdoc*/
        public int ExpectedQuantity { get; set; }

        /*inheritdoc*/
        public int AlreadyPrinted { get; set; }

        /// <summary>
        /// Unused here please use AerotecObjects.
        /// </summary>
        public Dictionary<string, string> Objects { get; set; } = new Dictionary<string, string>();

        public Dictionary<Keys, string> AerotecObjects { get; set; } = new Dictionary<Keys, string>();

        /*inheritdoc*/
        public string getJobStartMessage()
        {
            var message = getSizeMessage();
            if (AerotecObjects.ContainsKey(Keys.Anzahl))
            {
                message += Write(AerotecObjects[Keys.Anzahl]);
            }
            else
            {
                message += Write();
            }
            message += "^0*ENDJOB []" + Constants.vbCrLf
                    + $"^0*ENDLJSCRIPT []" + Constants.vbCrLf;
            return message;
        }

        private string getSizeMessage()
        {
            switch (FontSize)
            {
                case FontSizeEnum.ISO1_5x3:
                    return "^0*BEGINLJSCRIPT [()]" + Constants.vbCrLf
                            + $"^0*JLPAR [ 60 1 0 3 1000 {Rotation} 0 {EncoderResolution} 00:00 0 30000 0 0 1000]" + Constants.vbCrLf
                    + $"^0*BEGINJOB [ 0 () ]" + Constants.vbCrLf
                            + $"^0*JOBPAR [ {Delay} 0 0 {GetDistanceBetweenDots(FontSize, MachineType)} 0 0 0 1 1 0 -1 () 1 1 55000 0 9 0 1 0 100 0 1 0]" + Constants.vbCrLf;
                case FontSizeEnum.ISO1_7x5:
                    return "^0*BEGINLJSCRIPT [()]" + Constants.vbCrLf
                            + $"^0*JLPAR [ 90 1 0 3 1000 {Rotation} 0 {EncoderResolution} 00:00 0 30000 0 0 1000]" + Constants.vbCrLf
                            + $"^0*BEGINJOB [ 0 () ]" + Constants.vbCrLf
                            + $"^0*JOBPAR [ {Delay} 0 0 {GetDistanceBetweenDots(FontSize, MachineType)} 0 0 0 1 1 0 -1 () 1 1 55000 0 16 0 1 0 100 0 1 0 ]" + Constants.vbCrLf;
                case FontSizeEnum.ISO1_9x7:
                    return "^0*BEGINLJSCRIPT [()]" + Constants.vbCrLf
                            + $"^0*JLPAR [ 90 1 0 3 1000 {Rotation} 0  {EncoderResolution}  00:00 0 30000 0 0 1000]" + Constants.vbCrLf
                            + $"^0*BEGINJOB [ 0 () ]" + Constants.vbCrLf
                            + $"^0*JOBPAR [  {Delay}  0 0 {GetDistanceBetweenDots(FontSize, MachineType)} 0 0 0 1 1 0 -1 () 1 1 55000 0 21 0 1 0 100 0 1 0 ]" + Constants.vbCrLf;
                default:

                    return "";
            }
        }

        /// <summary>
        /// Writes the basic message stirng that will be send to the machine.
        /// </summary>
        /// <param name="HTZ"></param>
        /// <param name="signature"></param>
        /// <param name="ANR"></param>
        /// <param name="BTIDX"></param>
        /// <param name="controllerId"></param>
        /// <returns></returns>
        public string Write()
        {
            var location = Setlocation(FontSize, MachineType);
            string bold = GetBold(FontSize, MachineType);
            switch (FontSize)
            {

                case FontSizeEnum.ISO1_5x3:
                    return  $"^0*OBJ [1 {location[0]} 11 0 (ISO1_7x5)  ({AerotecObjects[Keys.HTZ]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [2 {location[1]} 11 0 (ISO1_7x5)  ({AerotecObjects[Keys.Signature]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [3 {location[2]} 0 0 (ISO1_7x5)  ({AerotecObjects[Keys.ANR]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [4 {location[3]} 0 0 (ISO1_7x5)  ({AerotecObjects[Keys.BTIDX]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [5 {location[4]} 0 0 (ISO1_7x5)  ({AerotecObjects[Keys.ControllerId]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf;

                case FontSizeEnum.ISO1_7x5:
                    return  $"^0*OBJ [1 {location[0]} 13 0 (ISO1_7x5)  ({AerotecObjects[Keys.HTZ]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [2 {location[1]} 13 0 (ISO1_7x5)  ({AerotecObjects[Keys.Signature]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [3 {location[2]} 0 0 (ISO1_7x5)  ({AerotecObjects[Keys.ANR]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [4 {location[3]} 0 0 (ISO1_7x5)  ({AerotecObjects[Keys.BTIDX]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [5 {location[4]} 0 0 (ISO1_7x5)  ({AerotecObjects[Keys.ControllerId]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf;

                case FontSizeEnum.ISO1_9x7:
                    return  $"^0*OBJ [1 {location[0]} 16 0 (ISO1_9x7)  ({AerotecObjects[Keys.HTZ]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [2 {location[1]} 16 0 (ISO1_9x7)  ({AerotecObjects[Keys.Signature]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [3 {location[2]} 0 0 (ISO1_9x7)  ({AerotecObjects[Keys.ANR]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [4 {location[3]} 0 0 (ISO1_9x7)  ({AerotecObjects[Keys.BTIDX]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf
                            + $"^0*OBJ [5 {location[4]} 0 0 (ISO1_9x7)  ({AerotecObjects[Keys.ControllerId]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 ()]" + Constants.vbCrLf;
                default:

                    return "";
            }
        }

        /// <summary>
        /// Writes the final message to the machine. It has a final string.
        /// </summary>
        /// <param name="HTZ"></param>
        /// <param name="signature"></param>
        /// <param name="ANR"></param>
        /// <param name="BTIDX"></param>
        /// <param name="controllerId"></param>
        /// <param name="final"></param>
        /// <returns></returns>
        public string Write(string final)
        {
            var location = Setlocation(FontSize, MachineType);
            string bold = GetBold(FontSize, MachineTypeEnum.Neagra);
            switch (FontSize)
            {
                case FontSizeEnum.ISO1_5x3:
                    return  $"^0*OBJ [1 {location[0]} 22 0 (ISO1_7x5)  ({AerotecObjects[Keys.HTZ]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [2 {location[1]} 22 0 (ISO1_7x5)  ({AerotecObjects[Keys.Signature]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [3 {location[2]} 11 0 (ISO1_7x5)  ({AerotecObjects[Keys.ANR]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [4 {location[3]} 11 0 (ISO1_7x5)  ({AerotecObjects[Keys.BTIDX]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [5 {location[4]} 11 0 (ISO1_7x5)  ({AerotecObjects[Keys.ControllerId]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [6 0 0 0 (ISO1_7x5)  ({final}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf;
                case FontSizeEnum.ISO1_7x5:
                    return  $"^0*OBJ [1 {location[0]} 24 0 (ISO1_7x5)  ({AerotecObjects[Keys.HTZ]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [2 {location[1]} 24 0 (ISO1_7x5)  ({AerotecObjects[Keys.Signature]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [3 {location[2]} 12 0 (ISO1_7x5)  ({AerotecObjects[Keys.ANR]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [4 {location[3]} 12 0 (ISO1_7x5)  ({AerotecObjects[Keys.BTIDX]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [5 {location[4]} 12 0 (ISO1_7x5)  ({AerotecObjects[Keys.ControllerId]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [6 0 0 0 (ISO1_7x5)  ({final}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf;


                case FontSizeEnum.ISO1_9x7:
                    return  $"^0*OBJ [1 {location[0]} 32 0 (ISO1_9x7)  ({AerotecObjects[Keys.HTZ]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [2 {location[1]} 32 0 (ISO1_9x7)  ({AerotecObjects[Keys.Signature]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [3 {location[2]} 16 0 (ISO1_9x7)  ({AerotecObjects[Keys.ANR]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [4 {location[3]} 16 0 (ISO1_9x7)  ({AerotecObjects[Keys.BTIDX]} ) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [5 {location[4]} 16 0 (ISO1_9x7)  ({AerotecObjects[Keys.ControllerId]}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf
                            + $"^0*OBJ [6 0 0 0 (ISO1_9x7)  ({final}) 1 0 0 0 0 {bold} 0 0 0 0 ()  () 0 0 () ]" + Constants.vbCrLf;

                default:

                    return "";
            }
           
        }
        /// <summary>
        /// Sets up location for the machine dependeing of the Ink color and expected machine type.
        /// </summary>
        /// <param name="size">The <see cref="FontSizeEnum"/> used by the printer.</param>
        /// <param name="machineType">The <see cref="MachineTypeEnum"/> of the Ink.</param>
        /// <returns>Coordinates, and distance between dots.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private int[] Setlocation(FontSizeEnum size, MachineTypeEnum machineType)
        {
            switch (machineType)
            {
                case MachineTypeEnum.Alba:
                    switch (size)
                    {
                        case FontSizeEnum.ISO1_5x3:
                            return new int[] { 0, 107, 0, 70, 98 };
                        case FontSizeEnum.ISO1_7x5:
                            return new int[] { 0, 172, 0, 114, 158 };
                        case FontSizeEnum.ISO1_9x7:
                            return new int[] { 0, 242, 0, 162, 222 };
                        default:
                            throw new ArgumentOutOfRangeException(nameof(size));
                    }

                case MachineTypeEnum.Neagra:
                    switch (size)
                    {
                        case FontSizeEnum.ISO1_5x3:
                            return new int[] { 0, 92, 0, 60, 80 };
                        case FontSizeEnum.ISO1_7x5:
                            return new int[] { 0, 92, 0, 60, 82 };
                        case FontSizeEnum.ISO1_9x7:
                            return new int[] { 0, 128, 0, 84, 120 };
                        default:
                            throw new ArgumentOutOfRangeException(nameof(size));
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(machineType));
            }
        }

        /// <summary>
        /// Selects the correct bold based on the <see cref="FontSizeEnum"/> and the <see cref="MachineTypeEnum"/>.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="machineType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string GetBold(FontSizeEnum size, MachineTypeEnum machineType)
        {
            switch (machineType)
            {
                case MachineTypeEnum.Alba:
                    switch (size)
                    {
                        case FontSizeEnum.ISO1_5x3:
                            return "1 0 1";
                        case FontSizeEnum.ISO1_7x5:
                            return "1 1 2";
                        case FontSizeEnum.ISO1_9x7:
                            return "1 1 2";
                        default:
                            throw new ArgumentOutOfRangeException(nameof(size));
                    }

                case MachineTypeEnum.Neagra:
                    switch (size)
                    {
                        case FontSizeEnum.ISO1_5x3:
                            return "1 0 0";
                        case FontSizeEnum.ISO1_7x5:
                            return "1 0 0";
                        case FontSizeEnum.ISO1_9x7:
                            return "1 0 1";
                        default:
                            throw new ArgumentOutOfRangeException(nameof(size));
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(machineType));
            }
        }
        /// <summary>
        /// Set's up the distance between Dots based on the font size and Ink type.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="machineType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string GetDistanceBetweenDots(FontSizeEnum size, MachineTypeEnum machineType)
        {
            switch (machineType)
            {
                case MachineTypeEnum.Alba:
                    switch (size)
                    {
                        case FontSizeEnum.ISO1_5x3:
                            return "320";
                        case FontSizeEnum.ISO1_7x5:
                            return "380";
                        case FontSizeEnum.ISO1_9x7:
                            return "420";
                        default:
                            throw new ArgumentOutOfRangeException(nameof(size));
                    }

                case MachineTypeEnum.Neagra:
                    switch (size)
                    {
                        case FontSizeEnum.ISO1_5x3:
                            return "220";
                        case FontSizeEnum.ISO1_7x5:
                            return "320";
                        case FontSizeEnum.ISO1_9x7:
                            return "380";
                        default:
                            throw new ArgumentOutOfRangeException(nameof(size));
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(machineType));
            }
        }
    }
}
