// import timeago from 'timeago.js';
import { format, register } from 'timeago.js'

function timeAgoFormat(time) {   
    const hebrewLang = function (number, index) {
        return [
            ['זה עתה', 'עכשיו'],
            ['לפני %s שניות', 'בעוד %s שניות'],
            ['לפני דקה', 'בעוד דקה'],
            ['לפני %s דקות', 'בעוד %s דקות'],
            ['לפני שעה', 'בעוד שעה'],
            ['לפני %s שעות', 'בעוד %s שעות'],
            ['אתמול', 'מחר'],
            ['לפני %s ימים', 'בעוד %s ימים'],
            ['לפני שבוע', 'בעוד שבוע'],
            ['לפני %s שבועות', 'בעוד %s שבועות'],
            ['לפני חודש', 'בעוד חודש'],
            ['לפני %s חודשים', 'בעוד %s חודשים'],
            ['לפני שנה', 'בעוד שנה'],
            ['לפני %s שנים', 'בעוד %s שנים']
        ][index];
    };

    const englishLang = function (number, index) {
        return [
            ['just now', 'right now'],
            ['%s seconds ago', 'in %s seconds'],
            ['1 min ago', 'in 1 min'],
            ['%s mins ago', 'in %s mins'],
            ['1 hour ago', 'in 1 hour'],
            ['%s hours ago', 'in %s hours'],
            ['1 day ago', 'in 1 day'],
            ['%s days ago', 'in %s days'],
            ['1 week ago', 'in 1 week'],
            ['%s weeks ago', 'in %s weeks'],
            ['1 month ago', 'in 1 month'],
            ['%s months ago', 'in %s months'],
            ['1 year ago', 'in 1 year'],
            ['%s years ago', 'in %s years']
        ][index];
    };
    
    register('he', hebrewLang);
    register('en', englishLang);
    let locale = global.lang.toLowerCase() === 'he' ? 'he' : '';
    return format(time, locale);    
}

export default {
    timeAgoFormat
}