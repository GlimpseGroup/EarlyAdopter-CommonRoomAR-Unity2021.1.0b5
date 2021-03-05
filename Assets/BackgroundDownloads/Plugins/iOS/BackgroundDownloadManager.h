#import <Foundation/Foundation.h>
#import "BackgroundDownloadDelegate.h"

@interface BackgroundDownloadManager : NSObject {
    NSMutableDictionary *pendingDownloadTasks;
}

+ (BackgroundDownloadManager *) instance;

- (long) startDownload:(NSString *) url;
- (void) cancelDownloadTask:(long) identifier;

- (NSURLSessionTask *) getDownloadTask:(long) identifier;
- (NSURLSessionTask *) getDownloadTaskWithURL:(NSString *) url;

- (void) setDownloadPath:(NSString *) storagePath;
- (void) moveDownloadToDestination:(NSURL *) tempFileURL downloadedFilename:(NSString *) filename;

- (void) initDownloadSession;

@end