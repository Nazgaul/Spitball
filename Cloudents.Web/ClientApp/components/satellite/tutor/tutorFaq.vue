<template>
    <v-container class="tutor-faq-container">
        <v-layout column>
            <v-flex my-2><h1>ללמד שיעור פרטי און-ליין ולקבל שכר על שיעור מלא! קראו עד הסוף !!!</h1></v-flex>
            <v-flex my-2>ספיטבול היא המערכת הכי פופולרית בקרב סטודנטים במדינת ישראל. כיום רשומים למעלה מחצי מיליון סטודנטים מכל העולם וכ- 200 אלף סטודנטים מישראל.</v-flex>
            <v-flex my-2>לאחר למעלה מחצי שנה של פיתוח אנחנו משיקים, לראשונה בישראל, מערכת און ליין לשיעורים פרטיים. המערכת כוללת: תקשורת בזמן אמת, לוח כתיבה אינטראקטיבי, מערכת לכתיבת קוד, הקלדת פונקציות, שיתוף מסך, הקלטה של השיעור לחזרה ועוד... כמו שעשינו בעבר, מטרתינו ליצור סביבת לימודים שבה סטודנטים מלמדים סטודנטים אחרים. במערכת היום התנהלו למעלה מ- 570 שיעורים בשביעות רצון גבוהה מצד המורים והסטודנטים.</v-flex>
            <v-flex my-2>צפו בסרטון כדי להבין איך השיעור מתנהל בפועל:</v-flex>
            <v-flex my-2><iframe width="560" height="315" src="https://www.youtube.com/embed/JCTqWXYex7w" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe></v-flex>
            <v-flex my-2>היום יש דרישה עצומה לשיעורים פרטיים בפלטפורמה שלנו ולכן הביקוש למורים פרטיים עולה. יש לנו צוות אשר אחראי על תיאום פגישות וברגע שתרשמו נוסיף אתכם למאגר המורים על מנת לתאם לכם שיעור עם סטודנט במקצוע הלימוד שלכם, בזמנכם המתאים בלבד וללא כל התחייבות. אנו נדאג להעביר לכם את החומרים הרלוונטים לשיעור מהסטודנט כדי לבצע שיעור אפקטיבי ויעיל יותר.</v-flex>
            <v-flex my-2>בזמן ההשקה אנו מעוניינים להגדיל את כמות הסטודנטים שייקחו שיעורים פרטיים אונליין דרך הפלטפורמה שלנו בכך שישלמו על שיעור של שעה - 50 ש"ח בלבד! כמובן, אתם בתור המורים תקבלו את המחיר שאתם קבעתם - בניכוי 9% עמלה למערכת ספיטבול.</v-flex>
        </v-layout>
         <v-layout column>
            <v-flex my-2><h1>מדריך הרשמה כמורה/ מתרגל פרטי בספיטבול</h1></v-flex>
            <v-flex mb-2>(הדרכה בלשון זכר אך פונה לנשים וגברים כאחד)</v-flex>
            <v-flex my-2><h3>הרשמה כמורה פרטי/מתרגל למי שאינו רשום לספיטבול כסטודנט:</h3></v-flex>
            <v-flex my-2>
                <ol>
                <li>במידה ואתה רשום לספיטבול דלג לשלב הזה - <router-link to="/tutorFaq#registerTutor">רשום</router-link>.</li>    
                <li>הירשם לאתר ספיטבול - <a href="https://www.spitball.co/register" target="_blank">https://www.spitball.co/register</a>.</li>    
                <li>הרשמה כוללת אימייל + מספר טלפון.</li>    
                <li>לאחר ההרשמה בחר באפשרות - <b>מתרגל</b>.</li>    
                <div><img :src="`${require('./imgs/1.png')}`" style="width: 309.92px;height: 423.83px;" alt=""> </div>
                <li>בחר אילו קורסים או נושאים אתה מעוניין ללמד.</li>    
                <li>מלא את המידע הרלוונטי - כולל תמונה.</li>    
                <li>שים לב: תמיד תוכל לשנות ולעדכן את המידע.</li>    
                <div><img :src="`${require('./imgs/2.png')}`" style="width: 444.50px;height: 458.36px;" alt=""> </div>
                <li>זהו, סיימת! <img :src="`${require('./imgs/3.png')}`" style="width: 51.47px;height: 51.47px;"> במידה ונתקלת בבעיה, צור קשר - support@spitball.co</li>    
            </ol>
            </v-flex>
            <v-flex my-2><h3 id="registerTutor">הרשמה כמורה פרטי/מתרגל למי שרשום לספיטבול כסטודנט.</h3></v-flex>
            <v-flex my-2>
                <ol>
                <li>כנס לספיטבול עם האימייל שלך - <a href="https://www.spitball.co/register" target="_blank">https://www.spitball.co/signin</a></li>    
                <li>בעמוד הבית (לאחר הרשמה) לחץ על התמונה שלך</li>    
                <div><img :src="`${require('./imgs/4.png')}`" style="width: 198.56px;height: 398.83px;" alt=""> </div>
                <li>בדף הפרופיל לחץ על כפתור - הירשם כמתרגל</li>    
                <div><img :src="`${require('./imgs/5.png')}`" style="width: 500.79px;height: 299.83px;" alt=""> </div>
                <li>מלא את הפרטים כולל תמונה.</li>    
                <li>בחר את הקורסים שאתה מעוניין ללמד.</li>    
                <li>זהו, סיימת! <img :src="`${require('./imgs/3.png')}`" style="width: 51.47px;height: 51.47px;"> במידה ונתקלת בבעיה, צור קשר - support@spitball.co </li>    
            </ol>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {

}
</script>

<style lang="less">
.tutor-faq-container{
    /*rtl:ignore*/
    direction: rtl;
    max-width: 780px;
    font-size: 16px;
    color: #666666;
    h1{
        color:#1976d2;
    }
}

</style>
