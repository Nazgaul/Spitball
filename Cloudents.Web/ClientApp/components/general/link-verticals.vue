<template>
    <div>
        <ul>
            <li v-for="vertical in verticals">
                <router-link :to="{name:vertical.name}" :key="vertical.name">
                    <button type="button">
                        <div class="round-icon" :class="'bg-'+vertical.name">
                            <component v-bind:is="vertical.image"></component>
                        </div>
                        <span>{{vertical.name}}</span>
                    </button>
                </router-link>
            </li>         
        </ul>
    </div>
</template>


<script>
    //import vertical from './vertical.vue';
    import ask from './images/ask.svg';
    import book from './images/book.svg';
    import document from './images/document.svg';
    import flashcard from './images/flashcard.svg';
    import job from './images/job.svg';
    import purchase from './images/purchase.svg';
    import tutor from './images/tutor.svg';
    import { verticalsList as verticals} from '../data.js'



    //let selected = verticals[0];

    export default {
        components: {
            ask, book, document, flashcard, job, purchase, tutor
        },
        props: {
            changeCallback: { type: Function }
        },
        data() {          
            return {
                verticals: verticals,
                selected: verticals[0]
            }
        },
        methods: {
            change(vertical) {
                this.selected = vertical;
                this.changeCallback(vertical);
            }
        }
    };
</script>
<style src="./vertical-collection.less" lang="less" scoped></style>