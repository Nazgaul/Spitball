import ask from './images/ask.svg'
import book from './images/book.svg'
import document from './images/document.svg'
import flashcard from './images/flashcard.svg'
import job from './images/job.svg'
import purchase from './images/purchase.svg'
import tutor from './images/tutor.svg'


export default {
    name: 'vertical',
    components: {
        ask, book,document,flashcard,job,purchase,tutor
    },
props: ["currentView"],
    data() {
        return {
            
        }
    }
    // props: ['className', 'glyph', 'width', 'height'],
};