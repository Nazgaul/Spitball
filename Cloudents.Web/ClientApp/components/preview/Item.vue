<template>
    <div>Hi,{{id}}, {{itemName}}</div>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        data() {
            return {
                item:null
            }
        },
        methods: {
            ...mapActions(['getPreview'])
        }, 

        created() {
            this.getPreview({ type: 'item', id: this.id }).then(res => {
                console.log(res)
                this.item =res
            })
        },
        props: { id: { type: String }, itemName: {type:String}}
    }
</script>