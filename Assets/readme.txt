//------------------------------------------------------------------------------------------------------------
[Updates 2014-09-21]

- Nathalie and Justin from Microsoft kindly provided the updated code for Twitter API 1.1.
You can also check their github page for this project.
https://github.com/ProtossEngineering/TwitterAuth
//------------------------------------------------------------------------------------------------------------

[Let's Tweet in Unity!]

* This script package is developped in C#.

1. OAuth support with PIN. 
 The most difficult part of accessing Twitter API is dealing with OAuth (http://dev.twitter.com/pages/auth). 
 With this code package, you can easily get and save "access token & secret" of player's Twitter account.
 It allows you to access all Twitter functionalities requring authentication with OAuth.
  
2. POST statuses/update API support. 
 With OAuth support, I have implemented "POST statuses/update" API (http://dev.twitter.com/doc/post/statuses/update).
 It will let you tweet in your game like "I've just reached 10,000 points in UnityGame!".
 I think this API function is the reason why most games want to access Twitter.

3. Compatibility.
 For HTTP requests, I use Unity's WWW class only to support all platforms where Unity works.
 .Net WebRequest class or some other web supports are NOT used to make this package as cmpatible as WWW class.

4. Extensibility.
 Code is well structured for you to easily add more Twitter API functions just like "POST statuses/update" API support I have made.
 Just use GetHeaderWithAccessToken() function to fill HTTP request header and run Twitter API url.
 
5. Free. :D


 [How to run Demo]
 1. You need to register your app in Twitter to get cosumer key and secret.
    Go to this page for registration: http://dev.twitter.com/apps/new

    *** NATHALIE - YOU CAN USE oBWUO1QvBV6mFrgalSshRGbCw AND KaVKvpVTr2zZ3lvazpoRO0R11XmOcx9HnkYXxvTuKP4RsZa2l8 FOR THE KEY/SECRET.
    
 2. After registration, you should be able to see your app's "Consumer key" and "Consumer secret"
 
 3. Copy the two values.
  
 4. Select "Demo" game object in Hierarchy window.
 
 5. Paste the two values to "CONSUMER_KEY" and "CONSUMER_SECRET" respectively in Inspector window.
 
 6. Get PIN by clikcing the top button.
 
 7. Type your PIN in the first input and click "Enter PIN"
    Make it sure that your Twitter account name is appearing on the top button.
    
 8. Type whatever you want in the second input and click "Post Tweet"
    Go to your Twitter webpage and the tweet should be there.

*** All the key images are created by my dear friend, Ivan Ortega( robotivan@gmail.com ).
    
Thank you.
Young ( youngwook.yang@gmail.com )
  
 