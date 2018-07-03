> Client-side data encrypting means that your data are encrypted in your browser before sending to the cloud. 
This guaranteed that your data exist as plain text only until you close the application page in your browser. 
The password never transmitted to the server and we never ask it. This means that you need to enter your password 
every time when you want review your records. Also it means that you are responsible for protecting the password 
from being stolen. If you forgot or lost your password we will not be able to restore your data. Your data are 
belong only to you.

> For encryption the opensource library [**SJCL**](https://crypto.stanford.edu/sjcl/) is used developed at Stanford University.

## Limitations

> The application encrypts only textual parts of your records - the names of expenses and categories. 
It doesn't encrypt values. If it would be so the application wouldn't be able to do any numeric calculations: 
statistics, totals, estimations and so on.

## Look how encryption works

{% include video_encrypting.html %}

[How to protect expenses by encryption.](https://dvmorozov.github.io/expenses/how-to-protect-expenses-by-encryption)
