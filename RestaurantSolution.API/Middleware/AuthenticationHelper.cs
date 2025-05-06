/// <summary>
/// Static utility class that provides methods for encoding and decoding HTTP Basic Authentication credentials.
/// Implements the standard RFC 7617 Basic Authentication scheme.
/// </summary>
/// <remarks>
/// Provides two primary functions:
/// - Encrypt: Converts username and password into a properly formatted Basic Authentication header value
/// - Decrypt: Extracts username and password from an existing Basic Authentication header value
/// 
/// The Basic Authentication format follows the pattern:
/// 1. Concatenate username and password with a colon separator (username:password)
/// 2. Encode the resulting string with Base64
/// 3. Prefix with "Basic " to create the complete header value
/// 
/// Note: Basic Authentication sends credentials in Base64 encoding, which is NOT encryption.
/// This should be used only with HTTPS to ensure secure transmission of credentials.
/// </remarks>

namespace RestaurantSolution.API.Middleware
{
    public class AuthenticationHelper
    {
        public static string Encrypt(string username, string password)
        {
            // 1. Concatenate credentials with a ':'
            string credentials = $"{username}:{password}";
            
            // 2. Retrieve bytes from text
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
            
            // 3. Base64 encode credentials
            string encryptedCredentials = Convert.ToBase64String(bytes);
            
            // 4. Prefix credentials with 'Basic' and return
            return $"Basic {encryptedCredentials}";
        }
        
        public static void Decrypt(string encryptedHeader, out string username, out string password)
        {
            // 1. Extract the username and password from the value by splitting it on space,
            // as the value looks something like 'Basic am9obi5kb2U6VmVyeVNlY3JldCE='
            var auth = encryptedHeader.Split(new[] { ' ' })[1];
            
            // 2. Convert it form Base64 encoded text, back to normal text
            var usernameAndPassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(auth));
            
            // 3. Extract username and password, which are separated by a semicolon
            username = usernameAndPassword.Split(new[] { ':' })[0];
            password = usernameAndPassword.Split(new[] { ':' })[1];
        }
    }
}