<template>
    <form>
        <div class="section-icon"><component v-bind:is="section"></component></div>
        <input type="search" placeholder="bobo"/>
    </form>
</template>
<style src="./searchFilter.less" type="text/less" scoped></style>
<script>
    import ask from './../../../general/images/ask.svg'
    import flashcard from './../../../general/images/flashcard.svg'
    import job from './../../../general/images/job.svg'
    import book from './../../../general/images/book.svg'
    import document from './../../../general/images/document.svg'
    import tutor from './../../../general/images/tutor.svg'
    import purchase from './../../../general/images/purchase.svg'
    export default {
        props: ["section"],
        components: { ask, book, document, flashcard, job, purchase, tutor}
    }
</script>