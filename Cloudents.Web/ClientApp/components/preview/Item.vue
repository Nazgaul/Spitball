<template>
    <div>
        <slot name="header">
            <v-container class="pa-0 mb-3 mt-2">
                <v-layout row>
                    <v-flex>{{item.name}}</v-flex>
                    <item-actions></item-actions>
                    </v-layout>
                </v-container>
                    <v-layout row><v-flex>{{item.author}}</v-flex> <v-flex>{{item.date | mediumDate}}</v-flex></v-layout>
        </slot>
        <div class="page text-xs-center" v-for="(page,index) in item.preview" :key="index">
            <component :is="currentComponent" :src="page" :class="item.type+'Content'"></component>
        </div>
    </div>
</template>
<script>
    import itemActions from './itemActions.vue'
    import { mapActions } from 'vuex'
    export default {
        data() {
            var actions = [{ id: 'info', click: function () { } }, { id: 'download' }, { id: 'print' }, { id: 'more' }, { id: 'close' }]
            return {
                item: {},
                actions
            }
        },

        components: {
            itemActions
        },

        methods: {
            ...mapActions(['getPreview'])
        }, 

        created() {
            this.getPreview({ type: 'item', id: this.id }).then(({data:body})  => {
                this.item ={...body.details,preview:body.preview};
                let postfix=this.item.preview[0].split('?')[0].split('.');
                this.item.type=postfix[postfix.length-1];
            })
        },
        props: { id: { type: String }},

        computed: {
            currentComponent() {
                return this.item.type==="html"? "iframe":"img";
            }
        },

        filters: {
            mediumDate: function (value) {
                if (!value) return '';
                let date = new Date(value);
                return date.toLocaleString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
            }
        }
    }
</script>
<style lang="less">

    .page {
        width: 800px;
        padding: 0 20px;
        text-align: center;
        margin: 0 auto;
    }

    .svgContent, .imageContent {
        background: #fff;
        border-radius: 15px;
        max-width: 100%;
        height: 100%;
        vertical-align: middle;
        margin: 10px 0;
        box-shadow: 0 3px 8px rgba(0,0,0,.2);
    }
    .textContent, .linkContent {
        border: 0;
        height: ~"calc(100vh - 175px)";
        width: ~"calc(100% - 40px)";
        max-width: 760px;
        background: #fff;

        @media (max-width: 600px) {
            width: ~ "calc(100% - 20px)";
        }
    }
</style>