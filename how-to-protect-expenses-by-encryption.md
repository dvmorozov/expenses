---
layout: default
--- 

> Just after you open the application in browser the encryption is disabled. The indicator at the upper left corner shows this by yellow.

![Status indicator shows that data aren't decrypted/encrypted by yellow](https://dvmorozov.github.io/expenses/assets/images/2015-07-03_10h48_58.png)

> If the data were encrypted earlier, you will see substitution text instead of them. To set up decrypting password click on the indicator.

![Password page](https://dvmorozov.github.io/expenses/assets/images/2015-07-03_10h51_00.png)

> On the password page type decryption password in the first input field. Press the "Set and Decrypt" button. This is enough to view encrypted data but new data WILL NOT BE ENCRYPTED. The indicator shows this state by blue.

![Status indicator shows that data are decrypted by blue](https://dvmorozov.github.io/expenses/assets/images/2015-07-03_22h34_29.png)

> If you want to encrypt new data you must repeat the password in the second input field. After that press the "Set New Password" button. The indicator shows this state by green.

![Status indicator shows that data are decrypted/encrypted by green](https://dvmorozov.github.io/expenses/assets/images/2015-07-03_15h42_00.png)

> If you want to turn encryption off press the "Discard Password" button. This will reset the application into initial state when only unencrypted data are shown. You can proceed data input in this state but your data will not be encrypted. You can encrypt them later. To do that you shoud setup the password as described before, then open the data you want to encrypt in the browser and select the "Encrypt All"  menu. The data are encrypted solely in the browser, so they must be open before.

!["Encrypt All" menu](https://dvmorozov.github.io/expenses/assets/images/2015-07-03_22h37_15.png)

> In principle you can use different passwords for different parts of data but in any time only one password is used. At any moment you see only the part of data which current password unlocks. You are responsible for remembering and keeping in secret the information about which part of data are encrypted by which password.

## How to encrypt existing categories

{% include video_encrypt_existing_categories.html %}

[Client-side data encryption](https://dvmorozov.github.io/expenses/client-side-data-encryption)

{% include google_ads.html %}
