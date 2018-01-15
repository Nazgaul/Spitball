<template>
    <div class="item">
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
                return this.item.type==="html"? "iframe":"img";
                if (['link', 'text'].find((x) => x == type.toLowerCase()))return 'iframe'
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
<style src="./item.less" lang="less"></style>