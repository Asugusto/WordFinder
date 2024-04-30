using System.Collections.Concurrent;

namespace WordFinder
{
    /// <summary>
    /// Represents a class for finding words in a character matrix.
    /// </summary>
    public class WordFinder
    {
        private readonly char[,] matrix; // Character matrix
        private readonly int rows; // Number of rows in the matrix
        private readonly int cols; // Number of columns in the matrix

        /// <summary>
        /// Initializes a new instance of the WordFinder class with the specified matrix.
        /// </summary>
        /// <param name="matrix">The character matrix.</param>
        /// <exception cref="ArgumentNullException">Thrown when the matrix is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the matrix size exceeds 64x64 or when not all strings in the matrix have the same length.
        /// </exception>
        public WordFinder(IEnumerable<string> matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            rows = matrix.Count();
            cols = matrix.First().Length;

            if (rows > 64 || cols > 64)
            {
                throw new ArgumentException("The matrix size cannot exceed 64x64.");
            }

            if (matrix.Any(row => row.Length != cols))
            {
                throw new ArgumentException("All strings in the matrix must have the same length.");
            }


            //Populate the matrix
            this.matrix = new char[rows, cols];

            int i = 0;
            foreach (var row in matrix)
            {
                for (int j = 0; j < cols; j++)
                {
                    this.matrix[i, j] = row[j];
                }
                i++;
            }
        }

        /// <summary>
        /// Finds the top 10 most repeated words from the word stream found in the matrix.
        /// </summary>
        /// <param name="wordstream">The word stream.</param>
        /// <returns>The top 10 most repeated words.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the word stream is null.</exception>
        public IEnumerable<string> Find(IEnumerable<string> wordstream)
        {
            if (wordstream == null)
            {
                throw new ArgumentNullException(nameof(wordstream));
            }

            //Parallelize the search for better performance
            var wordCount = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(wordstream, word =>
            {
                foreach (var foundWord in FindWord(word))
                {
                    wordCount.AddOrUpdate(foundWord, 1, (_, count) => count + 1);
                }
            });

            return GetTopWords(wordCount);
        }

        /// <summary>
        /// Finds occurrences of a word in the matrix.
        /// </summary>
        /// <param name="word">The word to find.</param>
        /// <returns>A list of found words.</returns>
        private List<string> FindWord(string word)
        {
            List<string> foundWords = new List<string>();

            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < cols; j++)
                {
                    //if the current char it's the same that first char of the word 
                    //check if the word is present in horizontal or in vertical 
                    if (matrix[i, j] == word[0])
                    {
                        if (CheckHorizontal(word, i, j) || CheckVertical(word, i, j))
                        {
                            lock (foundWords)
                            {
                                foundWords.Add(word);
                            }
                            break;
                        }
                    }
                }
            });

            return foundWords;
        }

        private bool CheckHorizontal(string word, int row, int col)
        {
            int length = word.Length;

            //if the word exceeds the length of the matrix then discard the search
            if (col + length > cols)
            {
                return false;
            }

            for (int j = 0; j < length; j++)
            {
                if (matrix[row, col + j] != word[j])
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckVertical(string word, int row, int col)
        {
            int length = word.Length;

            //if the word exceeds the length of the matrix then discard the search
            if (row + length > rows)
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (matrix[row + i, col] != word[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the top 10 most repeated words.
        /// </summary>
        /// <param name="wordCount">The dictionary containing word frequencies.</param>
        /// <returns>The top 10 most repeated words.</returns>
        private static IEnumerable<string> GetTopWords(ConcurrentDictionary<string, int> wordCount)
        {
            return wordCount.OrderByDescending(pair => pair.Value)
                            .ThenBy(pair => pair.Key)
                            .Take(10)
                            .Select(pair => pair.Key);
        }
    }
}