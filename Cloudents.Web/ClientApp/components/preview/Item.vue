<template>
    <div>
        <slot name="header">
            <v-container class="pa-0 mb-3 mt-2">
                <v-layout row>
                    <v-flex>{{item.name}}</v-flex>
                    <item-actions></item-actions>
                    <v-flex><!--<span @click="$_actionClick(act.callback)" class="ml-4" v-for="act in actions"><component :is="act.id+'-icon'"></component></span>--></v-flex>
                    </v-layout>
                </v-container>
                    <v-layout row><v-flex>{{item.author}}</v-flex> <v-flex>{{item.date | mediumDate}}</v-flex></v-layout>
        </slot>
        <div class="page text-xs-center" v-for="(page,index) in item.blob" :key="index">
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
            ...mapActions(['getPreview']),
            $_actionClick(act) {
                
            }
        }, 

        created() {
            this.getPreview({ type: 'item', id: this.id }).then(res => {
                console.log(res)
                this.item =res
            })
        },
        props: { id: { type: String }},

        computed: {
            currentComponent() {
                let type = this.item.type;
                if (['image', 'svg'].find((x) => x == type.toLowerCase())) return 'img'
                if (['link', 'text'].find((x) => x == type.toLowerCase()))return 'iframe'
            }
        },

        filters: {
            mediumDate: function (value) {
                if (!value) return '';
                var date = new Date(value)
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

    .imgContent {
        background: #fff;
        border-radius: 15px;
        max-width: 100%;
        height: 100%;
        vertical-align: middle;
        margin: 10px 0;
        box-shadow: 0 3px 8px rgba(0,0,0,.2);
    }
    .iframeContent {
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