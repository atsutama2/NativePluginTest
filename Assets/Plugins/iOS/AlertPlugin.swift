public class AlertPlugin : NSObject {

    @objc static func showAlert() {
        let alertView = UIAlertController(title: "Title", message: "AlertMessage", preferredStyle: UIAlertController.Style.alert)

        let okAction = UIAlertAction(title: "OK", style: UIAlertAction.Style.default, handler:nil)
        let cancelButton = UIAlertAction(title: "Cancel", style: UIAlertAction.Style.cancel, handler:nil)

        alertView.addAction(okAction)
        alertView.addAction(cancelButton)

        UIApplication.shared.keyWindow?.rootViewController?.present(alertView, animated: true, completion: nil)
    }
}
