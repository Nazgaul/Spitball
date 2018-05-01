<template>
    <div class="uni" :key="item.id" @click="$_universitySelected(item)">
        <div class="image uni-icon elevation-1" :style="`background-image: url('${item.image}')`"></div>
        <div class="uni-name text-sm-center text-xs-left">{{item.name}}</div>
    </div>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        props: { item: { type: Object, required: true } },
        methods: {
            ...mapActions(['updateUniversity']),
            $_universitySelected(val) {
                this.$ga.event("Uni_Select",val.name);
                this.updateUniversity(val);
            }
        }
    }
</script>