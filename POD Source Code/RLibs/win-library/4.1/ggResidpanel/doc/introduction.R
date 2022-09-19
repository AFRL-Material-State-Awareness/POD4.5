## ----setup, include = FALSE----------------------------------------------
knitr::opts_chunk$set(collapse = TRUE, comment = "#>", fig.align = "center",
                      message = FALSE, fig.height = 4, fig.width = 6)

## ----echo = FALSE--------------------------------------------------------
# Load libraries
library(dplyr)
library(ggplot2)

## ---- eval = FALSE-------------------------------------------------------
#  devtools::install_github("goodekat/ggResidpanel")

## ------------------------------------------------------------------------
library(ggResidpanel)

## ------------------------------------------------------------------------
str(penguins)

## ------------------------------------------------------------------------
table(penguins$bird)

## ----echo = FALSE--------------------------------------------------------
penguins %>%
  mutate(bird = forcats::fct_recode(bird, 
                                    "Penguin 1" = "1", "Penguin 2" = "2",
                                    "Penguin 3" = "3", "Penguin 4" = "4", 
                                    "Penguin 5" = "5", "Penguin 6" = "6", 
                                    "Penguin 7" = "7", "Penguin 8" = "8", 
                                    "Penguin 9" = "9")) %>%
ggplot(aes(x = depth, y = heartrate, color = bird)) + 
  geom_point() + 
  facet_wrap( ~ bird) + 
  theme_bw() + 
  scale_color_manual(values = wesanderson::wes_palette("Zissou1", 9, 
                                                       type = "continuous")[1:9]) +
  labs(x = "Depth (m)", y = "Heart Rate (beats per minute)", color = "Penguin") + 
  theme(legend.position = "none")

## ----echo = FALSE--------------------------------------------------------
penguins %>%
  mutate(bird = forcats::fct_recode(bird, 
                                    "Penguin 1" = "1", "Penguin 2" = "2",
                                    "Penguin 3" = "3", "Penguin 4" = "4", 
                                    "Penguin 5" = "5", "Penguin 6" = "6", 
                                    "Penguin 7" = "7", "Penguin 8" = "8", 
                                    "Penguin 9" = "9")) %>%
ggplot(aes(x = duration, y = heartrate, color = bird)) + 
  geom_point() + 
  facet_wrap( ~ bird) + 
  theme_bw() + 
  scale_color_manual(values = wesanderson::wes_palette("Zissou1", 9, 
                                                       type = "continuous")[1:9]) +
  labs(x = "Duration (min)", y = "Heart Rate (beats per minute)", color = "Penguin") + 
  theme(legend.position = "none")

## ------------------------------------------------------------------------
library(lme4)
penguin_model <- lmer(heartrate ~ depth + duration + (1|bird), data = penguins)

## ------------------------------------------------------------------------
summary(penguin_model)

## ----fig.height = 3, fig.width = 5---------------------------------------
resid_panel(penguin_model)

## ----fig.height = 3, fig.width = 6---------------------------------------
resid_panel(penguin_model, plots = "all")

## ----fig.height = 2------------------------------------------------------
resid_interact(penguin_model, plots = c("resid", "qq"))

## ----fig.height = 3, fig.width = 5---------------------------------------
resid_xpanel(penguin_model)

## ----fig.height = 3, fig.width = 5---------------------------------------
resid_xpanel(penguin_model, yvar = "response")

## ------------------------------------------------------------------------
penguin_model_log <- lmer(log(heartrate) ~ depth + duration + (1|bird), 
                       data = penguins)
penguin_model_log2 <- lmer(log(heartrate) ~ depth + duration + I(duration^2) + (1|bird), 
                       data = penguins)

## ----fig.width = 5, fig.height = 3---------------------------------------
resid_compare(models = list(penguin_model, 
                            penguin_model_log, 
                            penguin_model_log2),
              plots = c("resid", "qq"),
              smoother = TRUE,
              qqbands = TRUE,
              title.opt = FALSE)

## ------------------------------------------------------------------------
penguin_tree <- rpart::rpart(heartrate ~ depth + duration, data = penguins)

## ------------------------------------------------------------------------
penguin_tree_pred <- predict(penguin_tree)
penguin_tree_resid <- penguins$heartrate - penguin_tree_pred

## ----fig.height = 2, fig.width = 4.5-------------------------------------
resid_auxpanel(residuals = penguin_tree_resid, 
               predicted = penguin_tree_pred, 
               plots = c("resid", "index"))

