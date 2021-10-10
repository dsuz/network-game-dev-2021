using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;

/// <summary>
/// ユーザー管理をするコンポーネント
/// ユーザー登録・ログイン・ユーザー情報更新・パスワードのリセット・ログアウトの機能を提供する
/// 適当なオブジェクトに追加して使う
/// カスタムデータには JSON など文字列にシリアライズしたデータをやり取りすることを想定している
/// </summary>
public class UserManager : MonoBehaviour
{
    /// <summary>会員登録時にユーザーIDを入力するフィールド</summary>
    [SerializeField] InputField m_userIdForRegistration;
    /// <summary>会員登録時にパスワードを入力するフィールド</summary>
    [SerializeField] InputField m_passwordForRegistration;
    /// <summary>ログイン時にユーザーIDを入力するフィールド</summary>
    [SerializeField] InputField m_userIdForLogin;
    /// <summary>ログイン時にパスワードを入力するフィールド</summary>
    [SerializeField] InputField m_passwordForLogin;
    /// <summary>ユーザー情報を表示する時にユーザーIDが表示されるフィールド</summary>
    [SerializeField] InputField m_userIdForUserInfo;
    /// <summary>ユーザー情報を表示・更新する時にメールアドレスの出入力に使われるフィールド</summary>
    [SerializeField] InputField m_emailForUserInfo;
    /// <summary>ユーザー情報を表示・更新する時にカスタムデータの出入力に使われるフィールド</summary>
    [SerializeField] InputField m_customDataForUserInfo;
    /// <summary>カスタムデータのキー</summary>
    [SerializeField] string m_customDataKey = "CustomData";
    /// <summary>パスワードリセット時にメールアドレスを入力するフィールド</summary>
    [SerializeField] InputField m_emailForResetPassword;

    void Start()
    {
        // セッションが残っていたら、ユーザー情報を表示する
        NCMBUser user = NCMBUser.CurrentUser;
        if (user != null)
        {
            InitUserInfo(user);
        }
    }

    /// <summary>
    /// ユーザーを登録する
    /// ボタンクリックで呼ばれるための関数
    /// </summary>
    public void Register()
    {
        string userId = m_userIdForRegistration.text;
        string password = m_passwordForRegistration.text;
        Register(userId, password);
    }

    /// <summary>
    /// ユーザーを登録する
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    void Register(string userId, string password)
    {
        NCMBUser user = new NCMBUser();
        user.UserName = userId;
        user.Password = password;
        user.SignUpAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError("Failed to register: " + e.ErrorMessage);
            }
            else
            {
                Debug.Log("Registration success.");
            }
        });
    }

    /// <summary>
    /// ログインする
    /// ボタンクリックで呼ばれるための関数
    /// </summary>
    public void Login()
    {
        string userId = m_userIdForLogin.text;
        string password = m_passwordForLogin.text;
        Login(userId, password);
    }

    /// <summary>
    /// ログインする
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    void Login(string userId, string password)
    {
        NCMBUser.LogInAsync(userId, password, (NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError("Failed to login: " + e.ErrorMessage);
            }
            else
            {
                NCMBUser user = NCMBUser.CurrentUser;
                Debug.Log("Login successfully. User: " + user.UserName);
                InitUserInfo(user);
            }
        });
    }

    /// <summary>
    /// ユーザー情報表示パネルを初期化する
    /// </summary>
    /// <param name="user">ユーザーオブジェクト</param>
    void InitUserInfo(NCMBUser user)
    {
        if (user != null)
        {
            // カスタムデータはキーがない可能性があるため、キーの存在を確認してからデータを取り出す
            string customData = "";
            if (user.ContainsKey(m_customDataKey) && user[m_customDataKey] != null)
            {
                customData = user[m_customDataKey].ToString();
            }

            InitUserInfo(user.UserName, user.Email, customData);
        }
        else
        {
            InitUserInfo("", "", "");
        }
    }

    /// <summary>
    /// ユーザー情報表示パネルを初期化する
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="customData"></param>
    void InitUserInfo(string userName, string email, string customData)
    {
        m_userIdForUserInfo.text = userName;
        m_emailForUserInfo.text = email;
        m_customDataForUserInfo.text = customData;
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// ボタンクリックで呼ばれるための関数
    /// </summary>
    public void UpdateUserInfo()
    {
        string email = m_emailForUserInfo.text;
        string customData = m_customDataForUserInfo.text;

        UpdateUserInfo(email, customData);
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// （パスワードは NCMB の仕様で更新できない）
    /// </summary>
    /// <param name="email"></param>
    /// <param name="customData"></param>
    void UpdateUserInfo(string email, string customData)
    {
        NCMBUser user = NCMBUser.CurrentUser;

        if (user == null)
        {
            Debug.LogWarning("Not logged in. Log in first.");
            return;
        }

        if (email != "")
        {
            user.Email = email;
        }

        // カスタムデータはキーがない可能性があるため、キーがある場合はデータを更新し、キーがない場合はキーとデータのペアを追加する
        if (user.ContainsKey(m_customDataKey))
        {
            user[m_customDataKey] = customData;
        }
        else
        {
            Debug.LogFormat("Key [{0}] is not found. Add key...", m_customDataKey);
            user.Add(m_customDataKey, customData);
        }

        user.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError("Failed to save: " + e.ErrorMessage);
            }
            else
            {
                Debug.Log("Saved successfully.");
            }
        });
    }

    /// <summary>
    /// ログアウトする
    /// </summary>
    public void Logout()
    {
        NCMBUser.LogOutAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError("Failed to logout: " + e.ErrorMessage);
            }
            else
            {
                Debug.Log("Logout successfully.");
                InitUserInfo(null);
            }
        });
    }

    /// <summary>
    /// パスワードをリセットする
    /// 事前にユーザー情報にパスワードを追加しておく必要がある
    /// </summary>
    public void ResetPassword()
    {
        NCMBUser.RequestPasswordResetAsync(m_emailForResetPassword.text, (NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError("Failed to send email:" + e.ErrorMessage);
            }
            else
            {
                Debug.Log("Sent email for reset password.");
            }
        });
    }
}
