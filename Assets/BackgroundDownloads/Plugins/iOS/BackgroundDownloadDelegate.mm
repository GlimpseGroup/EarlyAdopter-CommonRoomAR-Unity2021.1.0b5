#import <Foundation/Foundation.h>
#import "BackgroundDownloadDelegate.h"
#import "BackgroundDownloadManager.h"

@implementation BackgroundDownloadDelegate
 
- (void)URLSession:(NSURLSession *)session
      downloadTask:(NSURLSessionDownloadTask *)downloadTask
 didResumeAtOffset:(int64_t)fileOffset
expectedTotalBytes:(int64_t)expectedTotalBytes
{
}

- (void)URLSession:(NSURLSession *)session
      downloadTask:(NSURLSessionDownloadTask *)downloadTask
      didWriteData:(int64_t)bytesWritten
 totalBytesWritten:(int64_t)totalBytesWritten
totalBytesExpectedToWrite:(int64_t)totalBytesExpectedToWrite
{
}

- (void)URLSession:(NSURLSession *)session
      downloadTask:(NSURLSessionDownloadTask *)downloadTask
didFinishDownloadingToURL:(NSURL *)location
{
    [[BackgroundDownloadManager instance] moveDownloadToDestination:location downloadedFilename:downloadTask.response.suggestedFilename];
}

- (void)URLSession:(NSURLSession *)session 
              task:(NSURLSessionTask *)task 
didCompleteWithError:(NSError *)error
{
}

@end