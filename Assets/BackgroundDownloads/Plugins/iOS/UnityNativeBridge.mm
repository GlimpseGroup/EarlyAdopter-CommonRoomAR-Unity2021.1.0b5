#include "UnityNativeBridge.h"
#include "BackgroundDownloadDelegate.h"
#include "BackgroundDownloadManager.h"

void init(const char *storagePath)
{
    [[BackgroundDownloadManager instance] setDownloadPath:[NSString stringWithUTF8String:storagePath]];
}

long startDownload(const char *url)
{
    return [[BackgroundDownloadManager instance] startDownload:[NSString stringWithUTF8String:url]];   
}

void cancelDownload(long id)
{
    [[BackgroundDownloadManager instance] cancelDownloadTask:id];
}

long getDownloadTask(const char* url)
{
    NSURLSessionTask *task = [[BackgroundDownloadManager instance] getDownloadTaskWithURL:[NSString stringWithUTF8String:url]];
    
    if (task == NULL)
    {
        // TODO: log ??
        
        return -1;
    }
    
    return task.taskIdentifier;
}

float getDownloadProgress(long id)
{
    NSURLSessionTask *task = [[BackgroundDownloadManager instance] getDownloadTask:id];
    
    if (task == NULL)
    {
        return 0.0f;
    }
    
    double received = (double)[task countOfBytesReceived];
    double total = (double)[task countOfBytesExpectedToReceive];
    
    return (total == 0) ? 0.0f : received / total;
}

int getDownloadStatus(long id)
{
    NSURLSessionTask *task = [[BackgroundDownloadManager instance] getDownloadTask:id];
    
    if (task == NULL)
    {
        return NSURLSessionTaskStateCanceling;
    }

    return [task state];
}

const char *getError(long id)
{
    NSURLSessionTask *task = [[BackgroundDownloadManager instance] getDownloadTask:id];
    
    if (task == NULL)
    {
        return "";
    }

    if (task.error == NULL)
    {
        return "";
    }

    return [[[task error] localizedDescription] UTF8String];
}