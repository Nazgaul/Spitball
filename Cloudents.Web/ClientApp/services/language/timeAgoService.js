import timeago from 'timeago.js';

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
    timeago.register('he', hebrewLang);
    let timeAgoRef = timeago();
    let locale = global.lang.toLowerCase() === 'he' ? 'he' : '';
    return timeAgoRef.format(time, locale);    
}

export default {
    timeAgoFormat
}