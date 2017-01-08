using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Storage;
using Os_Simulator.Enums;

namespace Os_Simulator.Components
{
    /// <summary>
    /// Loads programs, creates pcbs and assigns them unique ids
    /// </summary>
    public class FileLoader
    {
        private int _idCount = 0;

        /// <summary>
        /// Parses the command and any params from a single line of input and calls the corresponding function
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Operation convertLineToOperation(string line)
        {
            var op = line.Split(' ');

            var type = op[0].ToUpper();

            var types = new string[] { "CALCULATE", "I/O", "YIELD", "OUT" };

            var foundType = types.FirstOrDefault<string>(s => type.Equals(s));

            switch (foundType)
            {
                case "CALCULATE":
                    return new Operation(OperationType.CALCULATE, System.Convert.ToInt32(op[1]));
                case "I/O":
                    var io = new IoBurst();
                    var o = new Operation(OperationType.IO, io.GenerateIoBurst());
                    o.IsIo = true;
                    o.IoType = op.Length == 2 ? op[1] : getRandomIO();

                    return o;
                case "YIELD":
                    return new Operation(OperationType.YIELD);
                case "OUT":
                    return new Operation(OperationType.OUT);
                default:
                    //something is wrong
                    return null;
            }
        }

        /// <summary>
        /// Get a random io type. Used when no io type is passed with an io operation
        /// </summary>
        /// <returns></returns>
        private string getRandomIO()
        {
            string[] ios = new string[] { "KEYBOARD", "MOUSE", "PRINTER", "MONITOR", "NETWORK", "HDD" };

            Random r = new Random();
            int i = r.Next(0, ios.Length);
            return ios[i];
        }

        /// <summary>
        /// Load the program in and assign it to whichever queue is appropriate
        /// </summary>
        /// <param name="filename">Name of the file(program) to load</param>
        /// <returns></returns>
        public async Task<ProcessControlBlock> LoadFile(string filename)
        {
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///program_files/" + filename));
                using (var inputStream = await file.OpenReadAsync())
                using (var classicStream = inputStream.AsStreamForRead())
                using (var streamReader = new StreamReader(classicStream))
                {
                    var counter = 0;
                    int memRequirement = 0;
                    List<Operation> operations = new List<Operation>();
                    while (streamReader.Peek() >= 0)
                    {

                        if (counter == 0)
                        {
                            memRequirement = Convert.ToInt32(streamReader.ReadLine());
                        }
                        else {
                            operations.Add(convertLineToOperation(streamReader.ReadLine()));
                        }

                        counter++;
                    }

                    ProcessControlBlock newProcess = new ProcessControlBlock(State.NEW, memRequirement, operations);
                    newProcess.PId = this._idCount;
                    this._idCount++;

                    return newProcess;
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
            
        }
     
    }
}
