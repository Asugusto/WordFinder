namespace WordFinder.Tests
{
    [TestClass]
    public class WordFinderTests
    {
        private readonly List<string> matrix = ["rain", "cold", "wind"];
        private readonly List<string> wordstream = ["chill", "cold", "wind", "weather", "rain", "snow"];

        [TestMethod]
        public void Find_ReturnsTop10Words_WhenMatrixContainsWords()
        {
            //Arrange
            var wordFinder = new WordFinder(matrix);

            //Act
            var result = wordFinder.Find(wordstream);

            //Assert
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains("rain"));
            Assert.IsTrue(result.Contains("cold"));
            Assert.IsTrue(result.Contains("wind"));
        }

        [TestMethod]
        public void Find_ReturnsTop10Words_WhenMatrixContainsWordsVertically()
        {
            //Arrange
            var matrix = new List<string> { "cccc", "hhhh", "iiii","llll","llll" };
            var wordFinder = new WordFinder(matrix);

            //Act
            var result = wordFinder.Find(wordstream);

            //Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Contains("chill"));
        }

        [TestMethod]
        public void Find_ReturnsEmpty_WhenMatrixDoesNotContainWords()
        {
            //Arrange
            var matrix = new List<string> { "aaaaa", "bbbbb", "ccccc" };
            var wordFinder = new WordFinder(matrix);

            //Act
            var result = wordFinder.Find(wordstream);

            //Assert
            Assert.AreEqual(0,result.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "The matrix size cannot exceed 64x64.")]
        public void Constructor_ThrowsArgumentException_WhenMatrixSizeExceeds64x64()
        {
            //Arrange
            var matrix = Enumerable.Repeat(new string('a', 65), 65).ToList();
            var wordFinder = new WordFinder(matrix);

            //Act

            //Assert
            //ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "All strings in the matrix must have the same length.")]
        public void Constructor_ThrowsArgumentException_WhenMatrixStringsHaveDifferentLengths()
        {
            //Arrange
            var matrix = new List<string> { "aaa", "bb", "ccc" };
            var wordFinder = new WordFinder(matrix);

            //Act

            //Assert
            //ExpectedException
        }
    }
}