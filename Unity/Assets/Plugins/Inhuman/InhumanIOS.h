#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>
#import <MessageUI/MFMailComposeViewController.h>
#import <MediaPlayer/MediaPlayer.h>


@interface InhumanIOS : NSObject <MFMailComposeViewControllerDelegate, UIAlertViewDelegate>
{
    UIViewController * currentViewController;  
}

- (void)Popup: (const char *) title: (const char *) message: (const char *) buttonTitle;
- (void)PopupYesNo: (const char *) title: (const char *) message;
- (void)ComposeEmail: (UIViewController *)viewController :(const char *)to: (const char *)subject: (const char *)body;

@end

