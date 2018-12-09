<template>
    <div class="page">
    <nav>

    </nav>
    <main>
    <section class="section">One</section>
    <section class="section">           
        <div class="section-part-left">dsgfsdfds</div>
        <div class="section-part-right">
            <div class="carousel-wrap">
            <v-carousel  v-if="cardList"
                        :height="180"
                        delimiter-icon="sbf-circle-solid"
                        :prev-icon="isRtl ? 'sbf-arrow-right-carousel' : 'sbf-arrow-left-carousel'"
                        :next-icon="isRtl ?  'sbf-arrow-left-carousel': 'sbf-arrow-right-carousel'">
                <v-carousel-item  v-for="(item, index) in cardList" src="" :key="index">
                        <question-card 
                        :fromCarousel="true"                        
                        :cardData="item"
                        :typeAnswer="false" 
                        :suggestion="true"
                        ></question-card>
                </v-carousel-item>
                </v-carousel>
            </div>
         
        <extended-text-area
                uploadUrl="/api/upload/ask"
                v-model="textAreaValue"
                :actionType="actionType"
                :error="errorTextArea"
                :isAttachVisible="false"
        ></extended-text-area>
        </div>
    </section>
    <section class="section">
<div class="section-part-left">Section part left</div>
        <div class="section-part-right">
       <new-question></new-question>
        </div>

    </section>
    <section>Four</section>
    <section>Five</section>
    <section>Six</section>
    <section>Seven</section>
    </main>
    </div>
</template>


<style src="./landingPage.less" scoped lang="less"></style>
<script src="./landingPage.js"></script>