<template>
    <component is="slot">
        <v-container v-for="(strip,index) in strips" class="strip" :class="strip.class">
            <v-layout row justify-center align-center class="strip-content">
                <v-flex class="image-wrapper">
                    <div class="images">
                        <img :src="require(`./img/${strip.image}`)" />
                        <div class="floating-icons">
                            <component v-for="(image,index) in strip.floatingImages" :is="image" :class="'icon-' + (index+1)"></component>
                        </div>
                    </div>
                </v-flex>
                <v-flex class="description pa-4">
                    <v-layout class="strip-title" row align-center justify-start>
                        <component :is="strip.titleIcon"></component>
                        <h3 :style="{'color':strip.titleColor}">{{strip.title}}</h3>
                    </v-layout>
                    <p class="strip-text">{{strip.text}}</p>

                </v-flex>
            </v-layout>
        </v-container>
    </component>
</template>



<style src="./horizontalStrip.less" lang="less" scoped></style>
<script>
    import stripDocumentIcon from "./svg/strip-documents-icon.svg";
    import bagIcon from "./svg/bag-icon.svg";
    import notebookIcon from "./svg/notebook-icon.svg";
    import laptopIcon from "./svg/laptop-icon.svg";

    import stripTutorIcon from "./svg/strip-tutor-icon.svg"
    import discussionIcon from "./svg/discussion-icon.svg"
    import tutorIcon from "./svg/tutor-icon.svg"
    import studentLaptopIcon from "./svg/student-laptop-icon.svg"

    import stripFoodDealsIcon from "./svg/strip-food-deals-icon.svg"
    import shoppingBagsIcon from "./svg/shopping-bags-icon.svg"
    import headsetIcon from "./svg/headset-icon.svg"
    import pizzaIcon from "./svg/pizza-icon.svg"

    import stripTextbooksIcon from "./svg/strip-textbooks-icon.svg"
    import bookClosedIcon from "./svg/book-closed-icon.svg"
    import bookStackIcon from "./svg/book-stack-icon.svg"
    import bookOpenIcon from "./svg/book-open-icon.svg"

    import stripFlashcardsIcon from "./svg/strip-flashcards-icon.svg"
    import flashcardPenIcon from "./svg/flashcard-pen-icon.svg"
    import flashcardQuestionIcon from "./svg/flashcard-question-icon.svg"
    import flashcardGroupIcon from "./svg/flashcard-group-icon.svg"

    import stripAskQuestionIcon from "./svg/strip-ask-question-icon.svg"
    import askQuestionMountainIcon from "./svg/ask-question-mountain-icon.svg"
    import askQuestionHouseIcon from "./svg/ask-question-house-icon.svg"
    import askQuestionRocketIcon from "./svg/ask-question-rocket-icon.svg"

    import stripJobsIcon from "./svg/strip-jobs-icon.svg"
    import jobsGraphIcon from "./svg/jobs-graph-icon.svg"
    import jobsChemistryIcon from "./svg/jobs-chemistry-icon.svg"
    import jobsSlideIcon from "./svg/jobs-slide-icon.svg"

    export let strips =
        {

            documents: {
                class: "documents",
                image: "strip-documents.png",
                floatingImages: [laptopIcon, notebookIcon, bagIcon],
                titleIcon: stripDocumentIcon,
                title: "Study documents",
                titleColor: "#320161",
                text: "Spitball curates content for you from the best sites on the web. The documents populate based on student ratings and are filtered by your school, classes, and preferences."
            },

            tutors: {
                class: "tutors",
                image: "strip-tutors.png",
                floatingImages: [tutorIcon, discussionIcon,studentLaptopIcon],
                titleIcon: stripTutorIcon,
                title: "Tutors",
                titleColor: "#52ab15",
                text: "Spitball has teamed up with the most trusted tutoring services. All of our online and in-person tutors are highly qualified experts with educations from the best universities around the world."
            },

            foodDeals: {
                class: "food-deals",
                image: "strip-food-deals.png",
                floatingImages: [shoppingBagsIcon, headsetIcon, pizzaIcon],
                titleIcon: stripFoodDealsIcon,
                title: "Food and Deals",
                titleColor: "#2cbfa5",
                text: "Find and discover exclusive, short-term offers from restaurants around your campus."
            },

            textbooks: {
                class: "textbooks",
                image: "strip-textbook-icon.png",
                floatingImages: [bookClosedIcon, bookStackIcon, bookOpenIcon],
                titleIcon: stripTextbooksIcon,
                title: "Textbooks",
                titleColor: "#8c3fbf",
                text: "Spitball finds you textbooks at the best prices by simultaneously searching multiple websites. Compare prices to buy, rent, and sell back your books."
            },


            flashcards: {
                class: "flashcards",
                image: "strip-flashcards.png",
                floatingImages: [flashcardPenIcon, flashcardQuestionIcon, flashcardGroupIcon],
                titleIcon: stripFlashcardsIcon,
                title: "Flashcards",
                titleColor: "#f14d4d",
                text: "Search millions of study sets and improve your grades by studying with flashcards."
            },


            askQuestion: {
                class: "ask-question",
                image: "strip-ask-question.png",
                floatingImages: [askQuestionMountainIcon, askQuestionHouseIcon, askQuestionRocketIcon],
                titleIcon: stripAskQuestionIcon,
                title: "Ask a Question",
                titleColor: "#0455a8",
                text: "Ask any school related question and immediately get answers and information that relates specifically to you, your classes, and your university."
            },

            jobs: {
                class: "jobs",
                image: "strip-jobs.png",
                floatingImages: [jobsGraphIcon, jobsChemistryIcon, jobsSlideIcon],
                titleIcon: stripJobsIcon,
                title: "Jobs",
                titleColor: "#f5a623",
                text: "Easily find and apply to paid internships, part-time jobs and entry-level opportunities at thousands of Fortune 500 companies and startups."
            }

        }
    export default {
        data() {
            return {
                strips: strips
            }
        },
        components: {
            stripDocumentIcon,
            bagIcon,
            notebookIcon,
            laptopIcon
        }
    }
</script>