using System;
using System.Linq;
using System.Threading.Tasks;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class UploadFile
    {
        private readonly IntegrationFixture fixture;

        public UploadFile(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void UploadFile_Succeeds()
        {
            // Arrange
            const int FileSize = 500;
            const string FileName = "MyFile.bin";
            byte[] data = new byte[FileSize];

            // Act
            FileUploadResponse fileUploadResponse = null;
            using (var sync = new InSync(nameof(SlackClient.UploadFile)))
            {
                this.fixture.UserClient.UploadFile(c =>
                    {
                        fileUploadResponse = c;
                        Assert.True(c.ok);
                        sync.Proceed();
                    },
                    data, FileName, new[] {this.fixture.Config.TestChannel});
            }

            // Assert
            using (var sync = new InSync(nameof(SlackClient.UploadFile)))
            {
                this.fixture.UserClient.GetFileInfo(c =>
                {
                    Assert.True(c.ok);
                    Assert.Equal(FileSize, c.file.size);
                    Assert.Equal(FileName, c.file.name);
                    Assert.Equal(new[] { this.fixture.Config.TestChannel}, c.file.channels);
                    sync.Proceed();
                }, fileUploadResponse.file.id);
            }
        }

        [Fact]
        public async Task UploadFileAsync_Succeeds()
        {
            // Arrange
            const int FileSize = 500;
            const string FileName = "MyFile.bin";
            byte[] data = new byte[FileSize];

            // Act
            var fileUploadResponse = await this.fixture.UserClientAsync.UploadFileAsync(data, FileName, new[] { this.fixture.Config.TestChannel });

            // Assert
            Assert.True(fileUploadResponse.ok);
            var fileInfoResponse = await this.fixture.UserClientAsync.GetFileInfoAsync(fileUploadResponse.file.id);
            Assert.True(fileInfoResponse.ok);
            Assert.Equal(FileSize, fileInfoResponse.file.size);
            Assert.Equal(FileName, fileInfoResponse.file.name);
            Assert.Equal(new[] { this.fixture.Config.TestChannel}, fileInfoResponse.file.channels);
        }
    }
}
