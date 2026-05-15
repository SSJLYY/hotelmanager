using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Infrastructure;
using System;

public static class EmailTemplate
{
    private static string BasicTemplate(string branchName, string logoUrl, string headerContent,
                                       string customerName, string mailContent, string levelColor = "#0066CC",
                                       string levelText = "低") => $@"
                                                                    <!DOCTYPE html>
                                                                    <html>
                                                                    <head>
                                                                        <meta charset='utf-8'>
                                                                        <style>
                                                                            body {{ font-family: 'Microsoft YaHei', Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                                                                            .container {{ max-width: 600px; margin: 0 auto; background: #f9f9f9; }}
                                                                            .brand-header {{ background: white; padding: 20px; border-bottom: 1px solid #eaeaea; }}
                                                                            .brand-container {{ display: flex; align-items: center; }}
                                                                            .brand-logo {{ width: 50px; height: 50px; margin-right: 15px; }}
                                                                            .brand-name {{ font-size: 24px; font-weight: bold; color: #2c3e50; }}
                                                                            .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; }}
                                                                            .content {{ background: white; padding: 30px; }}
                                                                            .info-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
                                                                            .info-table td {{ padding: 10px; border-bottom: 1px solid #eee; }}
                                                                            .info-table tr:last-child td {{ border-bottom: none; }}
                                                                            .label {{ color: #666; width: 150px; }}
                                                                            .value {{ font-weight: 500; }}
                                                                            .warning-box {{ background: #fff8e1; border-left: 4px solid {levelColor}; padding: 15px; margin: 20px 0; }}
                                                                            .level-badge {{ display: inline-block; padding: 3px 8px; background: {levelColor}; color: white; border-radius: 3px; font-size: 12px; }}
                                                                            .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #999; font-size: 12px; text-align: center; }}
                                                                            .unsubscribe-section {{ margin-top: 25px; padding: 15px; background: #f5f5f5; border-radius: 5px; text-align: center; }}
                                                                            .unsubscribe-text {{ font-size: 11px; color: #777; margin-bottom: 10px; }}
                                                                            .unsubscribe-button {{ display: inline-block; padding: 8px 16px; background: #999; color: white; text-decoration: none; border-radius: 3px; font-size: 11px; }}
                                                                            .unsubscribe-button:hover {{ background: #777; }}
                                                                            .company-info {{ font-size: 10px; color: #aaa; margin-top: 15px; }}
                                                                        </style>
                                                                    </head>
                                                                    <body>
                                                                        <div class='container'>
                                                                            <div class='brand-header'>
                                                                                <div class='brand-container'>
                                                                                    <img src='{logoUrl}' alt='TS酒店Logo' class='brand-logo'>
                                                                                    <div class='brand-name'>{branchName}</div>
                                                                                </div>
                                                                            </div>

                                                                            <div class='header'>
                                                                                <h1>{headerContent}</h1>
                                                                            </div>

                                                                            <div class='content'>
                                                                                <p>尊敬的{customerName} 先生/女士，您好：</p>

                                                                                {mailContent}

                                                                                <p>感谢您选择TS酒店，如有任何疑问，请随时联系我们的客服中心。</p>
            
                                                                                <!-- 取消订阅部分 -->
                                                                                <div class='unsubscribe-section'>
                                                                                    <div class='unsubscribe-text'>
                                                                                        如果您不希望继续接收此类通知邮件，可以点击下方按钮取消订阅：
                                                                                    </div>
                                                                                    <a href='https://tshotel.com/unsubscribe' class='unsubscribe-button'>取消订阅</a>
                                                                                    <div class='company-info'>
                                                                                        TS酒店管理有限公司 | 客户服务热线：400-xxx-xxxx | 地址：xxxxxxxxxx
                                                                                    </div>
                                                                                </div>

                                                                                <div class='footer'>
                                                                                    <p>此邮件为系统自动发送，请勿回复。</p>
                                                                                    <p>发送时间：{DateTime.Now:yyyy年MM月dd日 HH:mm}</p>
                                                                                    <p>© {DateTime.Now.Year} TS酒店管理系统</p>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </body>
                                                                    </html>";

    public static MailTemplate GetResetPasswordTemplate(string userName, string newPwd)
    {
        return new MailTemplate
        {
            Subject = LocalizationHelper.GetLocalizedString("Reset Password Notice", "重置密码通知"),
            Body = BasicTemplate(SystemConstant.BranchName.Code,
                SystemConstant.BranchLogo.Code,
                "密码重置通知",
                userName,
                $@"<p>{LocalizationHelper.GetLocalizedString(
                    $"Your password was reset at <strong>{DateTime.Now:yyyy/MM/dd HH:mm}</strong>. New password:",
                    $"您的密码已在<strong>{DateTime.Now:yyyy/MM/dd HH:mm}</strong>重置。新密码如下：")}
                </p>
                <p style='color: #666;'>{newPwd}</p>
                <p>{LocalizationHelper.GetLocalizedString(
                    "Please keep your password secure and change it after login.",
                    "请妥善保管密码，并在成功登录后修改为你能记住的密码！")}</p>")
        };
    }

    public static MailTemplate GetNewRegistrationTemplate(string userName, string newPassword)
    {
        return new MailTemplate
        {
            Subject = LocalizationHelper.GetLocalizedString("New Registration Notification", "新注册通知"),
            Body = BasicTemplate(SystemConstant.BranchName.Code,
                SystemConstant.BranchLogo.Code,
                "新用户注册通知",
                userName,
                $@"<p>{LocalizationHelper.GetLocalizedString(
                    $"You have successfully registered to the system on <strong>{DateTime.Now:yyyy/MM/dd}</strong>. Your account credentials are as follows:",
                    $"您已于<strong>{DateTime.Now:yyyy/MM/dd}</strong>新注册系统成功，账号密码如下：")}
                </p>
                <p style='color: #666;'>{newPassword}</p>
                <p>{LocalizationHelper.GetLocalizedString(
                    "Please keep your password secure and change it after login.",
                    "请妥善保管密码，并在成功登录后修改为你能记住的密码！")}</p>")
        };
    }

    public static MailTemplate GetUpdatePasswordTemplate(string userName, string newPassword)
    {
        return new MailTemplate
        {
            Subject = LocalizationHelper.GetLocalizedString("Update Password Notification", "更新密码通知"),
            Body = BasicTemplate(SystemConstant.BranchName.Code,
                SystemConstant.BranchLogo.Code,
                "密码更新通知",
                userName,
                $@"<p>{LocalizationHelper.GetLocalizedString(
                    $"Your password was updated at <strong>{DateTime.Now:yyyy/MM/dd HH:mm}</strong>. New password:",
                    $"您的密码已在<strong>{DateTime.Now:yyyy/MM/dd HH:mm}</strong>更新。新密码如下：")}
                </p>
                <p style='color: #666;'>{newPassword}</p>
                <p>{LocalizationHelper.GetLocalizedString(
                    "Please keep your password secure and change it after login.",
                    "请妥善保管密码！")}</p>")
        };
    }

    public static MailTemplate GetTwoFactorRecoveryCodeLoginAlertTemplate(string userName, string userIdentity, DateTime? loginTime = null)
    {
        var occurredAt = loginTime ?? DateTime.Now;
        var safeUserName = string.IsNullOrWhiteSpace(userName) ? "User" : userName;
        var safeIdentity = string.IsNullOrWhiteSpace(userIdentity) ? "-" : userIdentity;

        return new MailTemplate
        {
            Subject = LocalizationHelper.GetLocalizedString("Security alert: recovery code login", "安全提醒：检测到备用码登录"),
            Body = BasicTemplate(
                SystemConstant.BranchName.Code,
                SystemConstant.BranchLogo.Code,
                "账户安全提醒",
                safeUserName,
                $@"<p>{LocalizationHelper.GetLocalizedString("A login used a 2FA recovery code.", "检测到您的账号使用了 2FA 恢复备用码登录。")}</p>
                    <p>{LocalizationHelper.GetLocalizedString("Time", "时间")}：<strong>{occurredAt:yyyy-MM-dd HH:mm:ss}</strong></p>
                    <p>{LocalizationHelper.GetLocalizedString("User", "用户")}：{safeUserName} ({safeIdentity})</p>
                    <p>{LocalizationHelper.GetLocalizedString("If this was not you, please reset your password and rebind your authenticator immediately.", "如果不是本人操作，请立即修改密码并重新绑定验证器。")}</p>",
                "#FF6600",
                "中")
        };
    }

    public static MailTemplate SendReservationExpirationNotificationTemplate(string roomNo,
        string reservationChannel, string customerName, string roomType, DateTime endDate, int daysLeft)
    {
        string levelColor = daysLeft switch
        {
            <= 1 => "#FF0000",
            <= 3 => "#FF9900",
            _ => "#0066CC"
        };

        string levelText = daysLeft switch
        {
            <= 1 => "紧急",
            <= 3 => "高",
            _ => "低"
        };

        return new MailTemplate
        {
            Subject = LocalizationHelper.GetLocalizedString(
                $"[Reminder: Appointment about to expire] {roomType} - {roomNo}",
                $"【预约即将过期提醒】{roomType} - {roomNo}"),
            Body = BasicTemplate(SystemConstant.BranchName.Code,
                SystemConstant.BranchLogo.Code,
                "预约到期提醒",
                customerName,
                $@"<p>您通过 <strong>{reservationChannel}</strong> 预订的房间即将到期，请及时处理。</p>
                
                <table class='info-table'>
                    <tr>
                        <td class='label'>房间号：</td>
                        <td class='value'><strong>{roomNo}</strong></td>
                    </tr>
                    <tr>
                        <td class='label'>房型：</td>
                        <td class='value'>{roomType}</td>
                    </tr>
                    <tr>
                        <td class='label'>到期时间：</td>
                        <td class='value'>{endDate:yyyy年MM月dd日}</td>
                    </tr>
                    <tr>
                        <td class='label'>剩余天数：</td>
                        <td class='value'>
                            <strong style='color: {levelColor};'>{daysLeft} 天</strong>
                            <span class='level-badge'>{levelText}优先级</span>
                        </td>
                    </tr>
                </table>

                <div class='warning-box'>
                    <p><strong>重要提醒：</strong></p>
                    <p>请及时安排入住事宜，避免过期造成不便。如需更改，请提前联系前台或使用App办理相关手续。</p>
                </div>",
                levelColor,
                levelText)
        };
    }
}
