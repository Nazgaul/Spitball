<template>
    <div class="item">
        <slot name="header">
            <v-toolbar fixed class="item-toolbar elevation-0" height="80">
                <v-layout class="toolbar-content" column>
                    <v-flex>
                        <v-layout row align-center justify-space-between>
                            <div class="item-name">{{item.name}}</div>
                            <item-actions></item-actions>
                        </v-layout>
                    </v-flex>
                    <v-flex class="item-meta mt-2">
                        <v-layout row align-center justify-space-between>
                            <div class="author">{{item.author}} woop</div>
                            <div class="date">{{item.date | mediumDate}}</div>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-toolbar>
        </slot>
        <div class="item-content">
            <div class="page text-xs-center" v-for="(page,index) in item.preview" :key="index">
                <component class="page-content" :is="currentComponent" :src="page" :class="item.type+'-content'"></component>
            </div>
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
            this.getPreview({ type: 'item', id: this.id }).then(({ data: body }) => {
                this.item = { ...body.details, preview: body.preview };
                let postfix = this.item.preview[0].split('?')[0].split('.');
                this.item.type = postfix[postfix.length - 1];
            })
        },
        props: { id: { type: String } },

        computed: {
            currentComponent() {

                let type = 'image';
                if (['image', 'svg'].find((x) => x == type.toLowerCase())) return 'img'
                if (['link', 'text'].find((x) => x == type.toLowerCase())) return 'iframe'
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
<style src="./item.less" lang="less"></style>