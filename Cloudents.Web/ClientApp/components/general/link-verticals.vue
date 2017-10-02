<template>
    <div>
        <div v-for="(vertical,index) in verticals">
            <label v-if="isNewLabel(index)" class="uppper">{{vertical.asideLabel}}</label>
            <router-link :to="{name:vertical.name}" :key="vertical.name">
                    <button type="button" inline-flex>
                        <div class="round-icon" :class="'bg-'+vertical.name">
                            <component v-bind:is="vertical.image"></component>
                        </div>
                        <span>{{vertical.name}}</span>
                    </button>
           </router-link>
        </div>
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
            },
            isNewLabel: (index) => { return (index == 0 || verticals[index].asideLabel != (verticals[index - 1].asideLabel)) }
        }
    };
</script>
<style src="./vertical-collection.less" lang="less" scoped></style>