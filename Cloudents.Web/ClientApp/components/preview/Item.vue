<template>
    <div class="item">
        <div class="item-content">
            <div class="page text-xs-center" v-for="(page,index) in item.preview" :key="index">
                <component class="page-content elevation-1" :is="currentComponent" :src="page" :class="item.type+'-content'"></component>
            </div>
        </div>
    </div>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        data() {
            var actions = [{ id: 'info', click: function () { } }, { id: 'download' }, { id: 'print' }, { id: 'more' }, { id: 'close' }]
            return {
                item: {},
                actions
            }
        },

        methods: {
            ...mapActions(['getPreview','updateItemDetails'])
        },

        created() {
            this.getPreview({ type: 'item', id: this.id }).then(({ data: body }) => {
                this.item = { ...body.details, preview: body.preview };
                this.updateItemDetails({ details: body.details});
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
        }
    }
</script>
<style src="./item.less" lang="less"></style>