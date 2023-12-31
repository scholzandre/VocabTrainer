# VocabTrainer

A simple vocabulary trainer with an built in translator programmed in C# and WPF.
Your data is stored in json files in the corresponding folder. (Please do not touch, even if there are safety mechanisms in the code)

## What can you do in this app?

Long story short: You can learn your own entered vocabulary

Long story short:

You can create multiple lists in which you can enter and learn your vocabulary. In addition to creating these lists and entries, you can also manage and delete them. 

You can learn your vocabulary in four different learning modes:
- simple repetition of entries (reading)
- enter the corresponding translation
- multiple choice with one correct entry from a total of 5 entries
- multiple choice, where five entries are given and you have to connect the correct ones

One entry is required for the first two learning modes and at least five for the last two.

To track your progress, there is also an analysis window where you can see how many entries you have seen at least once, how many you already know and how many you have answered incorrectly.

You can also look up translations without using Google Translate or any other external translator, as there is an inbuilt translator which uses the mymemory API. (Use at your own risk as the quality of the translation is lower)

Due to my inability to choose good colour schemes, I will give you the option to change some elements of the GUI in the settings. 
By the way, in the settings you can select the learning modes and the lists you want to repeat.

Finally, it is also possible to mark words and repeat them by selecting the corresponding list.